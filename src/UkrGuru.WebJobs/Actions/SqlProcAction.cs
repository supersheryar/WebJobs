// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Models;

namespace UkrGuru.WebJobs.Actions
{
    public class SqlProcAction : BaseAction
    {
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var proc = More.GetValue("proc") ?? throw new ArgumentNullException("proc");

            var data = More.GetValue("data");

            var timeout = More.GetValue("timeout", (int?)null);

            var result_name = More.GetValue("result_name");

            _logger.LogDebug("SqlProcAction prepared", new { jobId = JobId, proc, data = Utils.StrUtils.ShortStr(data, 100), result_name, timeout });

            if (string.IsNullOrEmpty(result_name))
            {
                await DbHelper.ExecProcAsync($"WJb_{proc}", data, timeout);

                _logger.LogInformation("SqlProcAction done", new { jobId = JobId });
            }
            else
            {
                var result = await DbHelper.FromProcAsync($"WJb_{proc}", data, timeout);

                More[result_name] = result;

                _logger.LogInformation("SqlProcAction done", new { jobId = JobId, result = Utils.StrUtils.ShortStr(result, 100) });
            }
        }
    }
}