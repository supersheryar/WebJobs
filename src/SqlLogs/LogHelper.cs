// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Text.Json;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs;

public static class LogHelper
{
    public static LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

    public static async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Trace, title, more, cancellationToken);
    public static async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Debug, title, more, cancellationToken);
    public static async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default) =>
        await LogAsync(LogLevel.Information, title, more, cancellationToken);
    public static async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Warning, title, more, cancellationToken);
    public static async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Error, title, more, cancellationToken);
    public static async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Critical, title, more, cancellationToken);

    public static async Task LogAsync(LogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinLogLevel) return;

        try
        {
            _ = await DbHelper.ExecProcAsync("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) },
                  cancellationToken: cancellationToken);
        }
        catch { }
    }

    public static void LogTrace(string title, object? more = null) => Log(LogLevel.Trace, title, more);
    public static void LogDebug(string title, object? more = null) => Log(LogLevel.Debug, title, more);
    public static void LogInformation(string title, object? more = null) => Log(LogLevel.Information, title, more);
    public static void LogWarning(string title, object? more = null) => Log(LogLevel.Warning, title, more);
    public static void LogError(string title, object? more = null) => Log(LogLevel.Error, title, more);
    public static void LogCritical(string title, object? more = null) => Log(LogLevel.Critical, title, more);

    public static void Log(LogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)MinLogLevel) return;

        try { DbHelper.ExecProc("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) }); }
        catch { }
    }
}
