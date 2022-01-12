// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class BaseAction
    {
        private const string GOOD_RULE = "next";
        private const string FAIL_RULE = "fail";

        private const string GOOD_PREFIX = GOOD_RULE + "_";
        private const string FAIL_PREFIX = FAIL_RULE + "_";

        public int JobId { get; set; }
        public More More { get; set; }

        public BaseAction()
        {
            More = new More();
        }

        public virtual void Init(Job job)
        {
            job.ThrowIfNull(nameof(job));

            JobId = job.JobId;

            More.AddNew(job.JobMore);
            More.AddNew(job.RuleMore);
            More.AddNew(job.ActionMore);
        }

        public virtual async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);

            return true;
        }

        public virtual async Task NextAsync(bool execute_result, CancellationToken cancellationToken = default)
        {
            var next_prefix = execute_result ? GOOD_PREFIX : FAIL_PREFIX;

            var next_rule = More.GetValue(execute_result ? GOOD_RULE : FAIL_RULE);
            if (string.IsNullOrEmpty(next_rule)) return;

            var next_more = new More();

            foreach (var more in More.Where(item => item.Key.StartsWith(next_prefix)))
                next_more.Add(more.Key[next_prefix.Length..], more.Value);

            await LogHelper.LogDebugAsync("NextAsync", new { jobId = JobId, next_rule, next_more }, cancellationToken);

            var next_jobId = await DbHelper.FromProcAsync("WJbQueue_Ins",
                new { Rule = next_rule, RulePriority = (byte)Priorities.ASAP, RuleMore = next_more },
                cancellationToken: cancellationToken);

            await LogHelper.LogInformationAsync("NextAsync", new { jobId = JobId, result = "OK", next_jobId }, cancellationToken);
        }

        public static string? ShortStr(string? text, int maxLength) => (!string.IsNullOrEmpty(text)
            && text.Length > maxLength) ? string.Concat(text.AsSpan(0, maxLength), "...") : text;
    }
}
