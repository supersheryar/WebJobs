// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class ActionInput 
    {
        [Key]
        public int ActionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ActionName { get; set; }

        [Required]
        [StringLength(255)]
        public string ActionType { get; set; }

        public string ActionMore { get; set; }
    }
}
    