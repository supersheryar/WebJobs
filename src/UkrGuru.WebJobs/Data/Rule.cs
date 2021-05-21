﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class Rule: Action
    {
        [Key]
        [Display(Name = "Id")]
        public int RuleId { get; set; }

        [Display(Name = "Rule")]
        public string RuleName { get; set; }

        [Display(Name = "Priority")]
        public Priorities RulePriority { get; set; } = Priorities.Normal;

        [Display(Name = "More")]
        public string RuleMore { get; set; }
    }
}
    