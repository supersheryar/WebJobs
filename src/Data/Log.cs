// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class Log
    {
        [Key]
        [Display(Name = "Id")]
        public int LogId { get; set; }

        public DateTime? Logged { get; set; }

        public LogLevel LogLevel { get; set; }

        public string? Title { get; set; }

        public string? LogMore { get; set; }
    }
}