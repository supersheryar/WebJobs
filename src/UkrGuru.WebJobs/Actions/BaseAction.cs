// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.WebJobs.Models;
using UkrGuru.WebJobs.Services;

namespace UkrGuru.WebJobs.Actions
{
    public class BaseAction
    {
        public ILogger<Worker> _logger;

        public int JobId { get; set; }
        public More More { get; set; }

        public virtual void Init(Job job, ILogger<Worker> logger)
        {
            JobId = job.Id;

            More = new More();
            More.AddNew(job.MoreJson);
            More.AddNew(job.RuleMoreJson);
            More.AddNew(job.ActionMoreJson);

            _logger = logger;
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
        }

        public virtual async Task NextAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
        }
    }
}
