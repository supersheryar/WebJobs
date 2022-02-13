// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebJobsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddWebJobs(connString: hostContext.Configuration.GetConnectionString("SqlJsonConnection"),
                        logLevel: hostContext.Configuration.GetValue<LogLevel>("WJbSettings:LogLevel"),
                        nThreads: hostContext.Configuration.GetValue<int>("WJbSettings:NThreads"));
                });
    }
}