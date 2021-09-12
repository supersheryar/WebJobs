// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Linq;

namespace UkrGuru.WebJobs.Data
{
    public static class MoreExtensions
    {
        public static void AddNew([NotNull] this More more, string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            var items = JsonSerializer.Deserialize<More>(json);

            foreach (var item in from item in items
                                 where !more.ContainsKey(item.Key)
                                 select item)
            {
                more.Add(item.Key, item.Value);
            }
        }

        public static string GetValue([NotNull] this More more, [NotNull] string name)
        {
            _ = more.TryGetValue(name, out var value);
            return value;
        }
        public static int? GetValue([NotNull] this More more, [NotNull] string name, int? defaultValue)
        {
            string value = more.GetValue(name);
            return !string.IsNullOrEmpty(value) ? Convert.ToInt32(value) : defaultValue;
        }
        public static bool GetValue([NotNull] this More more, [NotNull] string name, bool defaultValue)
        {
            string value = more.GetValue(name);
            return !string.IsNullOrEmpty(value) ? Convert.ToBoolean(value) : defaultValue;
        }
    }
}