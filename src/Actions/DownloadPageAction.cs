// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class DownloadPageAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var url = More.GetValue("url").ThrowIfBlank("url");
        var filename = More.GetValue("filename") ?? "file.txt";
        var result_name = More.GetValue("result_name") ?? "next_body";
        await LogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, filename, result_name }, cancellationToken);

        Data.File file = new() { FileName = filename };

        using (HttpClient client = new())
        {
            file.FileContent = await client.GetByteArrayAsync(url, cancellationToken);
        }

        await file.CompressAsync(cancellationToken);

        var content = await DbHelper.FromProcAsync<string>("WJbFiles_Ins", file, cancellationToken: cancellationToken);

        if (!string.IsNullOrEmpty(result_name)) More[result_name] = content;

        await LogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", content }, cancellationToken);

        return true;
    }
}
