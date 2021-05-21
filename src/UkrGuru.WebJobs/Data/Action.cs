// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data
{
    public partial class Action 
    {
        [Key]
        public int ActionId { get; set; }

        public string ActionName { get; set; }

        public string ActionType { get; set; }

        public string ActionMore { get; set; }
    }
}
    