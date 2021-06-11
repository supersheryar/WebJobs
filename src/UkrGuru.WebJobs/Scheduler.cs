// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs
{
    public class Scheduler : BackgroundService
    {
        private readonly ILogger<Scheduler> _logger;

        public Scheduler(ILogger<Scheduler> logger) { 
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _ = Task.Run(async () => await CreateCronJobs(stoppingToken), stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(60 - DateTime.Now.Second), stoppingToken);
            }
        }

        protected virtual async Task CreateCronJobs(CancellationToken stoppingToken)
        {
            try
            {
                await DbHelper.ExecProcAsync("WJbQueue_InsCron", cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WJbQueue_InsCron Error", nameof(ExecuteAsync));
                await LogHelper.LogErrorAsync("WJbQueue_InsCron Error", new { errMsg = ex.Message });
            }
        }
    }
}