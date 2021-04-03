using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Models;
using UkrGuru.WebJobs.Utils;

namespace WebJobsDemo.Actions
{
    public class YourSqlProcAction : BaseAction
    {
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var proc = More.GetValue("proc") ?? throw new ArgumentNullException("proc");

            var data = More.GetValue("data");

            var timeout = More.GetValue("timeout", (int?)null);

            var result_name = More.GetValue("result_name");

            _logger.LogDebug("YourSqlProcAction prepared", new { jobId = JobId, proc, data = StrUtils.ShortStr(data, 100), result_name, timeout });

            if (string.IsNullOrEmpty(result_name))
            {
                await DbHelper.ExecProcAsync($"Your_{proc}", data, timeout);

                _logger.LogInformation("YourSqlProcAction done", new { jobId = JobId });
            }
            else
            {
                var result = await DbHelper.FromProcAsync($"Your_{proc}", data, timeout);

                More[result_name] = result;

                _logger.LogInformation("YourSqlProcAction done", new { jobId = JobId, result = StrUtils.ShortStr(result, 100) });
            }
        }
    }
}