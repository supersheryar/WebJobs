// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class RuleInput
    {
        [Key]
        public int RuleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RuleName { get; set; }

        [Required]
        public Priorities RulePriority { get; set; } = Priorities.Normal;

        public string RuleMore { get; set; }

        [Required]
        public int ActionId { get; set; }
    }
}
    