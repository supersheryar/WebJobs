// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class RunApiProcAction : BaseAction
    {
        public class ApiSettings
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var api_settings_name = More.GetValue("api_settings_name").ThrowIfBlank("api_settings_name");
            var api_settings = await DbHelper.FromProcAsync<ApiSettings>("WJbSettings_Get", api_settings_name, cancellationToken: cancellationToken);
            api_settings.Url.ThrowIfBlank(nameof(api_settings));

            var proc = More.GetValue("proc").ThrowIfBlank("proc");
            var data = More.GetValue("data");
            var body = More.GetValue("body");

            var result_name = More.GetValue("result_name");
            if (string.IsNullOrEmpty(result_name)) result_name = "next_data";

            await LogHelper.LogDebugAsync(nameof(RunApiProcAction), new { jobId = JobId, url = api_settings.Url, 
                proc, data = ShortStr(data, 200), body = ShortStr(body, 200), result_name });

            HttpMethod method = HttpMethod.Get;
            if (!string.IsNullOrEmpty(body))
            {
                method = HttpMethod.Post;
                data = "body";
            }

            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(api_settings.Url);
            httpClient.DefaultRequestHeaders.Add("ApiHoleKey", api_settings.Key);

            var requestUri = $"{proc}";
            if (!string.IsNullOrEmpty(data)) requestUri += $"?data={Uri.EscapeDataString(data)}";

            HttpRequestMessage requestMessage = new HttpRequestMessage(method, requestUri);

            if (!string.IsNullOrEmpty(body))
                requestMessage.Content = new StringContent(body, Encoding.UTF8);

            var httpResponse = await httpClient.SendAsync(requestMessage, cancellationToken);

            httpResponse.EnsureSuccessStatusCode();

            var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (responseBody != null && responseBody.StartsWith("Error:"))
                throw new Exception(responseBody.Replace("Error:", "").TrimStart());

            await LogHelper.LogInformationAsync(nameof(RunApiProcAction), new { jobId = JobId, result = ShortStr(responseBody, 200) });

            More[result_name] = responseBody;

            return true;
        }
    }
}