// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class BaseAction
    {
        private const string NEXT_RULE = "next";
        private const string FAIL_RULE = "fail";

        private const string NEXT_PREFIX = NEXT_RULE + "_";
        private const string FAIL_PREFIX = FAIL_RULE + "_";

        public int JobId { get; set; }
        public More More { get; set; }

        public virtual void Init(Job job)
        {
            JobId = job.JobId;

            More = new More();
            More.AddNew(job.JobMore);
            More.AddNew(job.RuleMore);
            More.AddNew(job.ActionMore);
        }

        public virtual async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);

            return true;
        }

        public virtual async Task NextAsync(bool execute_result, CancellationToken cancellationToken)
        {
            var next_prefix = execute_result ? NEXT_PREFIX : FAIL_PREFIX;

            var next_rule = More.GetValue(execute_result ? NEXT_RULE : FAIL_RULE);
            if (string.IsNullOrEmpty(next_rule)) return;

            var next_more = new More();
            foreach (var more in More)
                if (more.Key.StartsWith(next_prefix))
                    next_more.Add(more.Key[next_prefix.Length..], more.Value);

            await LogHelper.LogDebugAsync("NextAsync", (jobId: JobId, next_rule, next_more));

            var next_jobId = await DbHelper.FromProcAsync("WJbQueue_Ins",
                new { Rule = next_rule, Priority = (byte)Priorities.ASAP, MoreJson = next_more });

            await LogHelper.LogInformationAsync("NextAsync done", (jobId: JobId, result: "OK", next_jobId));
        }

        public string ShortStr(string text, int maxLength) => (!string.IsNullOrEmpty(text) && text.Length > maxLength) ? text.Substring(0, maxLength) + "..." : text;
    }
}
