// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace UkrGuru.WebJobs.Data
{
    public partial class File
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string FileName { get; set; }

        public byte[] FileContent { get; set; }
    }
}
    