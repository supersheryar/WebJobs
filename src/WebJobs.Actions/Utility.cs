// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace UkrGuru.WebJobs
{
    public class Utility
    {

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^([0-9a-zA-Z_]([-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", RegexOptions.IgnoreCase);
        }

        public static bool IsHtmlBody(string body)
        {
            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            // or Regex tagRegex = new Regex(@"<[^>]+>");
            return tagRegex.IsMatch(body);
        }
    }
}
