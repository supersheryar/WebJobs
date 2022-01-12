// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class File
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string? FileName { get; set; }

        public byte[]? FileContent { get; set; }
    }
}
    