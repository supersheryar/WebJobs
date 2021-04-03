// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Models
{
    public partial class Rule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public Priorities Priority { get; set; } = Priorities.Normal;

        [Required]
        [Display(Name = "Action")]
        public int ActionId { get; set; }

        [Display(Name = "Action")]
        public string ActionName { get; set; }

        public string MoreJson { get; set; }
    }
}
    