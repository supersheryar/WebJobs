// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebJobsServiceCollectionExtensions
    {
        public static void AddWebJobs(this IServiceCollection services, string connString, LogLevel logLevel = LogLevel.Debug, int nThreads = 4)
        {
            services.AddSqlJson(connString ?? throw new ArgumentNullException(nameof(connString)));

            LogHelper.MinLogLevel = logLevel;

            Assembly.GetExecutingAssembly().InitDb();

            if (nThreads <= 0) return;

            services.AddHostedService<Scheduler>();
            for (int i = 0; i < nThreads; i++)
            {
                services.AddSingleton<IHostedService, Worker>();
            }
        }
    }
}