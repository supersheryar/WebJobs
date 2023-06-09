﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json.Serialization;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// 
/// </summary>
public class RunApiProcAction : BaseAction
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var api_settings_name = More.GetValue("api_settings_name").ThrowIfBlank("api_settings_name");

        var api_settings = await DbHelper.ExecAsync<ApiSettings>("WJbSettings_Get", api_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(api_settings);

        var url = api_settings.Url;
        ArgumentNullException.ThrowIfNull(url);

        var proc = More.GetValue("proc");
        ArgumentNullException.ThrowIfNull(proc);

        var data = More.GetValue("data");

        var body = More.GetValue("body");

        var result_name = More.GetValue("result_name");
        if (string.IsNullOrEmpty(result_name)) result_name = "next_data";

        await DbLogHelper.LogDebugAsync(nameof(RunApiProcAction), new
        {
            jobId = JobId,
            url = api_settings.Url,
            proc,
            data = ShortStr(data, 200),
            body = ShortStr(body, 200),
            result_name
        }, cancellationToken);

        HttpMethod method = HttpMethod.Get;
        if (!string.IsNullOrEmpty(body))
        {
            method = HttpMethod.Post;
            data = "body";
        }

        using HttpClient httpClient = new();

        httpClient.BaseAddress = new Uri(url);
        httpClient.DefaultRequestHeaders.Add("ApiHoleKey", api_settings.Key);

        var requestUri = $"{proc}";
        if (!string.IsNullOrEmpty(data)) requestUri += $"?data={Uri.EscapeDataString(data)}";

        HttpRequestMessage requestMessage = new(method, requestUri);

        if (!string.IsNullOrEmpty(body))
            requestMessage.Content = new StringContent(body, Encoding.UTF8);

        var httpResponse = await httpClient.SendAsync(requestMessage, cancellationToken);

        httpResponse.EnsureSuccessStatusCode();

        var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (responseBody != null && responseBody.StartsWith("Error:"))
            throw new Exception(responseBody.Replace("Error:", "").TrimStart());

        await DbLogHelper.LogInformationAsync(nameof(RunApiProcAction), new { jobId = JobId, result = ShortStr(responseBody, 200) }, cancellationToken);

        if (!string.IsNullOrEmpty(result_name)) More[result_name] = responseBody;

        return true;
    }
}
