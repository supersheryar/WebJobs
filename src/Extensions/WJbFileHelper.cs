// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;
using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Data;

public static class WJbFileHelper
{
    public static async Task<string?> GetAsync(string? value, CancellationToken cancellationToken = default)
    {
        var file = await GetFileAsync(value, cancellationToken);

        if (file?.FileContent != null) return Encoding.UTF8.GetString(file.FileContent);

        return await Task.FromResult(null as string);
    }

    public static async Task<string?> SetAsync(string? value, CancellationToken cancellationToken = default)
    {
        return await SetFileAsync(value, "file.txt", false, cancellationToken);
    }

    public static async Task DelFileAsync(string? value, CancellationToken cancellationToken = default)
    {
        if (value != null && Guid.TryParse(value, out var guidvalue))
        {
            await DbHelper.ExecProcAsync("WJbFiles_Del", guidvalue, cancellationToken: cancellationToken);
        }
    }

    public static async Task<File?> GetFileAsync(string? value, CancellationToken cancellationToken = default)
    {
        Data.File? file = null;

        if (value != null && Guid.TryParse(value, out var guidvalue))
        {
            file = await DbHelper.FromProcAsync<Data.File>("WJbFiles_Get", guidvalue, cancellationToken: cancellationToken);

            if (file?.FileContent != null) await file.DecompressAsync(cancellationToken);
        }

        return await Task.FromResult(file);
    }

    public static async Task<string?> SetFileAsync(string? value, string? filename = default, bool safe = default, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 200)
        {
            File file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            await file.CompressAsync(cancellationToken);

            return await DbHelper.FromProcAsync<string>("WJbFiles_Ins", file, cancellationToken: cancellationToken);
        }

        return await Task.FromResult(value);
    }

    public static async Task CompressAsync(this File file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        using var memoryStream = new MemoryStream();

        using (var compressStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            await compressStream.WriteAsync(file.FileContent.AsMemory(0, file.FileContent.Length), cancellationToken);
        }

        file.FileContent = memoryStream.ToArray();
        file.FileName = $"{file.FileName ?? "file.txt"}.gzip";
    }

    public static async Task DecompressAsync(this File file, CancellationToken cancellationToken = default)
    {
        if (file?.FileName == null || !file.FileName.EndsWith(".gzip")) return;

        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        using var memoryStream = new MemoryStream(file.FileContent);

        using var outputStream = new MemoryStream();

        using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            await decompressStream.CopyToAsync(outputStream, cancellationToken);
        }

        file.FileContent = outputStream.ToArray();
        file.FileName = file.FileName[..^5];
    }
}