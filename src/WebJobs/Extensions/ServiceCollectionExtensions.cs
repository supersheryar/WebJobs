// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebJobsServiceCollectionExtensions
    {
        public static void AddWebJobs(this IServiceCollection services, string connString, LogLevel logLevel = LogLevel.Debug, int nThreads = 4)
        {
            services.AddSqlJson(connString.ThrowIfNull(nameof(connString)));

            LogHelper.MinLogLevel = logLevel;

            Assembly.GetAssembly(typeof(BaseAction)).InitDb();

            if (nThreads <= 0) return;

            services.AddHostedService<Scheduler>();

            for (int i = 0; i < nThreads; i++)
                services.AddSingleton<IHostedService, Worker>();
        }
    }
}