// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.SqlJson
{
    public static class LogHelper
    {
        public static LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

        public static async Task LogTraceAsync(string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(LogLevel.Trace, title, more, cancellationToken);
        public static async Task LogDebugAsync(string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(LogLevel.Debug, title, more, cancellationToken);
        public static async Task LogInformationAsync(string title, object more = null, CancellationToken cancellationToken = default) =>
            await LogAsync(LogLevel.Information, title, more, cancellationToken);
        public static async Task LogWarningAsync(string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(LogLevel.Warning, title, more, cancellationToken);
        public static async Task LogErrorAsync(string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(LogLevel.Error, title, more, cancellationToken);
        public static async Task LogCriticalAsync(string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(LogLevel.Critical, title, more, cancellationToken);

        public static async Task LogAsync(LogLevel logLevel, string title, object more = null, CancellationToken cancellationToken = default)
        {
            if ((byte)logLevel < (byte)MinLogLevel) return;

            try
            {
                _ = await DbHelper.ExecProcAsync("WJbLogs_Ins", new { logLevel, title, logMore = more is string ? more : JsonSerializer.Serialize(more) }, 
                      cancellationToken: cancellationToken);
            }
            catch { }
        }

        public static async Task LogTraceAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Trace, title, more, cancellationToken);
        public static async Task LogDebugAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Debug, title, more, cancellationToken);
        public static async Task LogInformationAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Information, title, more, cancellationToken);
        public static async Task LogWarningAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Warning, title, more, cancellationToken);
        public static async Task LogErrorAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Error, title, more, cancellationToken);
        public static async Task LogCriticalAsync(this SqlConnection connection, string title, object more = null, CancellationToken cancellationToken = default)
            => await LogAsync(connection, LogLevel.Critical, title, more, cancellationToken);

        public static async Task LogAsync(this SqlConnection connection, LogLevel logLevel, string title, object more = null, CancellationToken cancellationToken = default)
        {
            if ((byte)logLevel < (byte)MinLogLevel) return;

            try {
                _ = await connection.ExecProcAsync("WJbLogs_Ins", new { logLevel, title, logMore = more is string ? more : JsonSerializer.Serialize(more) },
                      cancellationToken: cancellationToken);
            }
            catch { }
        }
    }
}
