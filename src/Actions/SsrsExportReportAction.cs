// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class SsrsExportReportAction : BaseAction
{
    public class SsrsSettings
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    private string GetReportFormat(string? filename)
    {
        switch (Path.GetExtension(filename)?.ToLower())
        {
            case ".docx":
                return "WORDOPENXML";
            case ".xlsx":
                return "EXCELOPENXML";
            case ".pptx":
                return "PPTX";
            case ".pdf":
                return "PDF";
            case ".tif":
            case ".tiff":
                return "IMAGE";
            case ".mhtml":
                return "MHTML";
            case ".csv":
                return "CSV";
            case ".xml":
                return "XML";
            case ".atom":
                return "ATOM";
            default:
                return "EXCELOPENXML";
        }
    }

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var ssrs_settings_name = More.GetValue("ssrs_settings_name").ThrowIfBlank("ssrs_settings_name");

        var ssrs_settings = await DbHelper.FromProcAsync<SsrsSettings>("WJbSettings_Get", ssrs_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(ssrs_settings);

        var url = ssrs_settings.Url;
        ArgumentNullException.ThrowIfNull(url);

        var report = More.GetValue("report").ThrowIfBlank("report");
        url = url.Replace("{report}", HttpUtility.UrlPathEncode(report));

        var data = More.GetValue("data") ?? String.Empty;
        if (!string.IsNullOrEmpty(data)) url += $"&data={HttpUtility.UrlEncode(data)}";

        int timeout = More.GetValue("timeout", 10) ?? 10;

        var filename = More.GetValue("filename");
        url = url.Replace("{format}", GetReportFormat(filename));

        var result_name = More.GetValue("result_name");

        await LogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, filename, result_name }, cancellationToken);

        Data.File file = new() { FileName = filename };

        var handler = new HttpClientHandler();
        if (!string.IsNullOrEmpty(ssrs_settings.UserName) && !string.IsNullOrEmpty(ssrs_settings.Password))
        {
            handler.Credentials = new NetworkCredential(ssrs_settings.UserName, ssrs_settings.Password);
            handler.PreAuthenticate = true; 
        }

        var client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, timeout) };

        var response = await client.GetAsync(new Uri(url), cancellationToken);

        response.EnsureSuccessStatusCode();

        file.FileContent = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var guid = await file.SetAsync(cancellationToken);

        if (!string.IsNullOrEmpty(result_name)) More[result_name] = guid;

        await LogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", guid }, cancellationToken);

        return true;
    }
}
