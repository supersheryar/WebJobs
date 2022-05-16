﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebJobsServiceCollectionExtensions
    {
        public static void AddWebJobsDemo(this IServiceCollection services, string connectionString, LogLevel logLevel = LogLevel.Debug, int nThreads = 4)
        {
            services.AddWebJobs(connectionString, logLevel, nThreads);

            Assembly.GetExecutingAssembly().InitDb();
        }
    }
}