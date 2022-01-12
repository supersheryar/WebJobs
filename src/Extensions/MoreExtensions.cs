// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text.Json;

namespace UkrGuru.WebJobs.Data
{
    public static class MoreExtensions
    {
        public static void AddNew(this More more, string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            var items = JsonSerializer.Deserialize<More>(json);

            if (items == null) return;

            foreach (var item in items.Where(item => !more.ContainsKey(item.Key)))
                more.Add(item.Key, Convert.ToString(item.Value));
        }

        public static string? GetValue(this More more, string name, string? defaultValue = default)
        {
            return more.TryGetValue(name, out var value) ? Convert.ToString(value) : defaultValue;
        }

        public static bool? GetValue(this More more, string name, bool? defaultValue)
        {
            return bool.TryParse(more.GetValue(name), out bool value) ? value : defaultValue;
        }

        public static int? GetValue(this More more, string name, int? defaultValue)
        {
            return (int.TryParse(more.GetValue(name), out int value)) ? value : defaultValue;
        }
    }
}