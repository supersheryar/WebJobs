using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;

namespace CustomActions
{
    public class YourSqlProcAction : BaseAction
    {
        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var proc = More.GetValue("proc");
            if (string.IsNullOrWhiteSpace(proc)) throw new(nameof(proc));

            var data = More.GetValue("data");

            var timeout = More.GetValue("timeout", (int?)null);

            var result_name = More.GetValue("result_name");

            await LogHelper.LogDebugAsync("YourSqlProcAction", new { jobId = JobId, proc, data = ShortStr(data, 200), result_name, timeout });

            if (string.IsNullOrEmpty(result_name))
            {
                _ = await DbHelper.ExecProcAsync($"Your_{proc}", data, timeout, cancellationToken);

                await LogHelper.LogInformationAsync("YourSqlProcAction done", new { jobId = JobId });
            }
            else
            {
                var result = await DbHelper.FromProcAsync($"Your_{proc}", data, timeout, cancellationToken);

                More[result_name] = result;

                await LogHelper.LogInformationAsync("YourSqlProcAction done", new { jobId = JobId, result = ShortStr(result, 200) });
            }

            return true;
        }
    }
}