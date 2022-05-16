// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs;

public static class LogExtensions
{
    public static async Task LogTraceAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Trace, title, more, cancellationToken);
    public static async Task LogDebugAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Debug, title, more, cancellationToken);
    public static async Task LogInformationAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Information, title, more, cancellationToken);
    public static async Task LogWarningAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Warning, title, more, cancellationToken);
    public static async Task LogErrorAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Error, title, more, cancellationToken);
    public static async Task LogCriticalAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Critical, title, more, cancellationToken);

    public static async Task LogAsync(this SqlConnection connection, LogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)LogHelper.MinLogLevel) return;

        try
        {
            _ = await connection.ExecProcAsync("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) },
                  cancellationToken: cancellationToken);
        }
        catch { }
    }

    public static void LogTrace(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Trace, title, more);
    public static void LogDebug(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Debug, title, more);
    public static void LogInformation(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Information, title, more);
    public static void LogWarning(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Warning, title, more);
    public static void LogError(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Error, title, more);
    public static void LogCritical(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Critical, title, more);

    public static void Log(this SqlConnection connection, LogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)LogHelper.MinLogLevel) return;

        try { connection.ExecProc("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) }); }
        catch { }
    }
}
