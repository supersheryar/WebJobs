// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static bool InitDb(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var product_name = assembly.GetName().Name;
            var product_version = Convert.ToString(assembly.GetName().Version);
            // var product_version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            var db_version = null as string;
            try { db_version = DbHelper.FromProc<string?>($"WJbSettings_Get", product_name); } catch { }

            db_version ??= "1.0.0.0";
            if (db_version.CompareTo(product_version) < 0)
            {
                var version_file = $"{product_name}.Resources.{db_version}.sql";

                var resourceNames = assembly.GetManifestResourceNames()
                    .Where(s => s.EndsWith(".sql") && s.CompareTo(version_file) >= 0)
                    .OrderBy(s => s);

                foreach (var resourceName in resourceNames) assembly.ExecResource(resourceName);

                try { DbHelper.ExecProc($"WJbSettings_Set", new { Name = product_name, Value = product_version }); } catch { }
            }

            //try { DbHelper.ExecProc($"WJbQueue_FinishAll"); } catch { }

            return true;
        }
    }
}