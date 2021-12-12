﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class FillTemplateAction : BaseAction
    {
        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            string template_prefix = "template_", tvalue_prefix = "tvalue_";

            // @"[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"
            var tname_pattern = More.GetValue("tname_pattern").ThrowIfBlank("tname_pattern"); 

            var vals = new More();
            foreach (var more in More.Where(item => item.Key.StartsWith(tvalue_prefix)))
                vals.Add(more.Key[tvalue_prefix.Length..], more.Value);

            var templates = new More();
            foreach (var more in More.ToList())
            {
                if (!more.Key.StartsWith("template_")) continue;

                var tkey = more.Key[template_prefix.Length..];
                var template = Convert.ToString(more.Value);

                var vars = (from m in new Regex(tname_pattern).Matches(template) select m.Value).Distinct().ToArray();
                
                foreach (var key in from key in vars where vals.ContainsKey(key) select key)
                    template = template.Replace(key, vals.GetValue(key));

                await LogHelper.LogDebugAsync(nameof(FillTemplateAction), 
                    new { jobId = JobId, tkey, template = ShortStr(template, 200) });

                More[$"next_{tkey}"] = template;
            }

            await LogHelper.LogInformationAsync(nameof(FillTemplateAction), new { jobId = JobId, result = "OK" });

            return true;
        }
    }
}
