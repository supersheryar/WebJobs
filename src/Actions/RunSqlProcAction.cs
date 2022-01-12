// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class RunSqlProcAction : BaseAction
    {
        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var proc = More.GetValue("proc").ThrowIfBlank("proc");

            var data = More.GetValue("data");

            var timeout = More.GetValue("timeout", (int?)null);

            var result_name = More.GetValue("result_name");

            await LogHelper.LogDebugAsync(nameof(RunSqlProcAction), new { jobId = JobId, proc, data = ShortStr(data, 200), result_name, timeout }, cancellationToken);

            if (string.IsNullOrEmpty(result_name))
            {
                _ = await DbHelper.ExecProcAsync($"WJb_{proc}", data, timeout, cancellationToken);

                await LogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId }, cancellationToken);
            }
            else
            {
                var result = await DbHelper.FromProcAsync($"WJb_{proc}", data, timeout, cancellationToken);

                More[result_name] = result;

                await LogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId, result = ShortStr(result, 200) }, cancellationToken);
            }

            return true;
        }
    }
}