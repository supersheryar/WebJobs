// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs
{
    public class Worker : BackgroundService
    {
        private int _delay = 100;

        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await DbHelper.FromProcAsync<JobQueue>("WJbQueue_Start1st", cancellationToken: stoppingToken);
                    if (job?.JobId > 0)
                    {
                        var jobId = job.JobId; bool result = false;
                        try
                        {
                            var action = job.CreateAction();

                            if (action != null)
                            {
                                result = await action.ExecuteAsync(stoppingToken);

                                await action.NextAsync(result, stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;

                            //_logger.LogError(ex, $"Job #{jobId} crashed.", nameof(ExecuteAsync));
                            await LogHelper.LogErrorAsync($"Job #{jobId} crashed.", new { jobId, errMsg = ex.Message }, stoppingToken);
                        }
                        finally
                        {
                            _ = await DbHelper.ExecProcAsync("WJbQueue_Finish", new { JobId = jobId, JobStatus = result ? JobStatus.Completed : JobStatus.Failed }, 
                                    cancellationToken: stoppingToken);
                        }

                        _delay = 100;
                    }
                    else
                    {
                        if (_delay < 12800) _delay += 100;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker.ExecuteAsync Error", nameof(ExecuteAsync));
                    await LogHelper.LogErrorAsync("Worker.ExecuteAsync Error", new { errMsg = ex.Message }, stoppingToken);
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}
