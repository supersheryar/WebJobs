// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Models;

namespace UkrGuru.WebJobs.Actions
{
    public class BaseAction
    {
        private const string NEXT_PREFIX = "next_";
        private const string NEXT_RULE = "next_rule";

        public int JobId { get; set; }
        public More More { get; set; }

        public virtual void Init(Job job)
        {
            JobId = job.Id;

            More = new More();
            More.AddNew(job.MoreJson);
            More.AddNew(job.RuleMoreJson);
            More.AddNew(job.ActionMoreJson);
        }

        public virtual async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);

            return true;
        }

        public virtual async Task NextAsync(CancellationToken cancellationToken)
        {
            var next_rule = More.GetValue(NEXT_RULE);
            if (string.IsNullOrEmpty(next_rule)) return;

            var next_more = new More();
            foreach (var more in More)
            {
                if (more.Key.StartsWith(NEXT_PREFIX) && !more.Key.Equals(NEXT_RULE, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    next_more.Add(more.Key[NEXT_PREFIX.Length..], more.Value);
                }
            }

            await LogHelper.LogDebugAsync("NextAsync", (jobId: JobId, next_rule, next_more));

            var next_jobId = await DbHelper.FromProcAsync("WJbQueue_Ins",
                new { Rule = next_rule, Priority = (byte)Priorities.ASAP, MoreJson = next_more });

            await LogHelper.LogInformationAsync("NextAsync done", (jobId: JobId, result: "OK", next_jobId));
        }
    }
}
