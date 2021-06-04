// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs
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
                    if (job.JobId > 0)
                    {
                        var jobId = job.JobId; bool result = false;
                        try
                        {
                            var type = Type.GetType(job.ActionType) ?? Type.GetType($"UkrGuru.WebJobs.Actions.{job.ActionType}");

                            dynamic action = Activator.CreateInstance(type);

                            action.Init(job);

                            result = await action.ExecuteAsync(stoppingToken);

                            await action.NextAsync(result, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            result = false; _logger.LogError(ex, $"Job #{jobId} crashed.");
                            await LogHelper.LogErrorAsync($"Job #{jobId} crashed.", new { jobId, errMsg = ex.Message });
                        }
                        finally
                        {
                            _ = await DbHelper.ExecProcAsync("WJbQueue_Finish", jobId.ToString());
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
                    await LogHelper.LogErrorAsync("Worker.ExecuteAsync Error", new { errMsg = ex.Message });
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}
