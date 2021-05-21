// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace UkrGuru.WebJobs.Data
{
    public class More : Dictionary<string, string>
    {
    }

    public static class MoreExtensions
    {
        public static void AddNew([NotNull] this More more, string json)
        {
            if (String.IsNullOrWhiteSpace(json)) return;

            var items = JsonSerializer.Deserialize<More>(json);
            foreach (var item in items)
                if (!more.ContainsKey(item.Key)) 
                    more.Add(item.Key, item.Value);
        }

        public static string GetValue([NotNull] this More more, [NotNull] string name)
        {
            more.TryGetValue(name, out var value); 
            return value;
        }
        public static int? GetValue([NotNull] this More more, [NotNull] string name, int? defaultValue)
        {
            string value = more.GetValue(name);
            return !string.IsNullOrEmpty(value) ? Convert.ToInt32(value) : defaultValue;
        }
    }
} 