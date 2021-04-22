using Microsoft.Extensions.Logging;
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
        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var proc = More.GetValue("proc");
            if (string.IsNullOrWhiteSpace(proc)) throw new (nameof(proc));

            var data = More.GetValue("data");

            var timeout = More.GetValue("timeout", (int?)null);

            var result_name = More.GetValue("result_name");

            await LogHelper.LogDebugAsync("YourSqlProcAction", (jobId: JobId, proc, data: StrUtils.ShortStr(data, 100), result_name, timeout));

            if (string.IsNullOrEmpty(result_name))
            {
                await DbHelper.ExecProcAsync($"Your_{proc}", data, timeout);

                await LogHelper.LogInformationAsync("YourSqlProcAction done", new { jobId = JobId });
            }
            else
            {
                var result = await DbHelper.FromProcAsync($"Your_{proc}", data, timeout);

                More[result_name] = result;

                await LogHelper.LogInformationAsync("YourSqlProcAction done", (jobId: JobId, result: StrUtils.ShortStr(result, 100)));
            }

            return true;
        }
    }
}