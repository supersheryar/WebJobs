// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Models;

namespace UkrGuru.WebJobs.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private int _delay = 100;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await DbHelper.FromProcAsync<JobQueue>("WJbQueue_Start1st");
                    if (job.Id > 0)
                    {
                        try
                        {
                            _logger.LogInformation($"Job #{job.Id} started.");
                            
                            var type = Type.GetType(job.ActionType) ?? Type.GetType($"UkrGuru.WebJobs.Actions.{job.ActionType}");
                            
                            dynamic action = Activator.CreateInstance(type);

                            action.Init(job, _logger);

                            await action.ExecuteAsync(stoppingToken);

                            await action.NextAsync(stoppingToken);

                            _logger.LogInformation($"Job #{job.Id} finished.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Job #{job.Id} crashed.");
                        }
                        finally
                        {
                            await DbHelper.ExecProcAsync("WJbQueue_Finish", job.Id);
                        }

                        _delay = 100;
                    }
                    else
                    {
                        if (_delay < 25600) _delay += 100;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker.ExecuteAsync Error");
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}