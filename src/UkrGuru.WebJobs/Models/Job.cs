// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Models
{
    public class JobQueue : Job { }
    public class JobHistory : Job { }

    public class Job 
    {
        [Key]
        public int Id { get; set; }

        public Priorities Priority { get; set; } = Priorities.Normal;

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime Created { get; set; }

        public int RuleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime? Started { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime? Finished { get; set; }

        public string MoreJson { get; set; }

        public string RuleName { get; set; }

        public string RuleMoreJson { get; set; }

        public string ActionName { get; set; }

        public string ActionType { get; set; }

        public string ActionMoreJson { get; set; }
    }
}
    