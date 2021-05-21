// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static void InitDb(this Assembly assembly)
        {
            var product_name = assembly.GetName().Name;
            var product_version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            var db_version = "0.0.0";
            try { db_version = DbHelper.FromProcAsync($"WJbSettings_Get", new { Name = product_name }).Result; } catch { }

            if (db_version.CompareTo(product_version) < 0)
            {
                var version_file = $"{product_name}.Resources.{db_version ?? "0.0.0"}.sql";

                var files = assembly.GetManifestResourceNames().Where(s => s.EndsWith(".sql")).OrderBy(s => s);
                foreach (var file in files)
                    if (file.CompareTo(version_file) >= 0) 
                        assembly.ExecScript(file);

                try { DbHelper.ExecProcAsync($"WJbSettings_Set", new { Name = product_name, Value = product_version }).Wait(); } catch { }
            }

            try { DbHelper.ExecProcAsync($"WJbQueue_FinishAll").Wait(); } catch { }
        }
    }
}