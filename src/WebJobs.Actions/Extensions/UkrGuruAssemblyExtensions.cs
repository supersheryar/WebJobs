// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.IO;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class UkrGuruAssemblyExtensions
    {
        public static void ExecScript(this Assembly assembly, string resourceName)
        {
            var script = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new(stream)) { script = reader.ReadToEnd(); }
                    
            using SqlConnection connection = new(DbHelper.ConnString);
            connection.Open();
                            
            using SqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
    }
}
