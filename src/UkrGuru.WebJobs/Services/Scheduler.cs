// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Services
{
    public class Scheduler : BackgroundService
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IConfiguration _configuration;

        public Scheduler(ILogger<Scheduler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private DateTime _lastTime = DateTime.Today;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //if (string.IsNullOrEmpty(DbHelper.ConnString))
                    //{
                    //    DbHelper.ConnString = _configuration.GetConnectionString("SqlJsonConnection");
                    //}

                    if (_lastTime.AddMinutes(1) < DateTime.Now)
                    {
                        _lastTime = DateTime.Now;

                        try
                        {
                            await DbHelper.ExecProcAsync("WJbQueue_InsCron");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "WJbQueue_InsCron Error", nameof(ExecuteAsync));
                        }
                    }

                    await Task.Delay(16000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Scheduler Error", nameof(ExecuteAsync));
            }
        }
    }
}