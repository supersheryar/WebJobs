﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;

namespace UkrGuru.WebJobs.Data;

public static class FileExtensions
{
    public static async Task CompressAsync(this File file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        using var memoryStream = new MemoryStream();

        using (var compressStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            await compressStream.WriteAsync(file.FileContent, 0, file.FileContent.Length, cancellationToken);
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
        file.FileName = file.FileName[..^4];
    }
}