// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class Rule: Action
    {
        [Key]
        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public Priorities RulePriority { get; set; } = Priorities.Normal;

        public string RuleMore { get; set; }
    }
}
    