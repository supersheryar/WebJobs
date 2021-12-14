// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class RunApiHoleAction : BaseAction
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
            var api_settings = await DbHelper.FromProcAsync<ApiSettings>("WJbSettings_Get", new { Name = api_settings_name }, 
                    cancellationToken: cancellationToken);
            api_settings.Url.ThrowIfBlank(nameof(api_settings));

            //var proc = "View_Get";
            //var jsonData = @"{""View"":""Jobs"", ""Page"":""0"", ""Size"":""250000""}";

            var result_name = More.GetValue("result_name");
            if (string.IsNullOrEmpty(result_name)) result_name = "next_data";

            //More data = new(); data.AddNew(jsonData);
            //More data_c = new(); data_c.AddNew(jsonData); data_c.Add("Content", "");

            //await LogHelper.LogDebugAsync(nameof(RunApiProcAction), new { jobId = JobId, url, result_name });

            //using var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(api_settings.Url);
            //httpClient.DefaultRequestHeaders.Add("ApiHoleKey", api_settings.Key);

            var content = api_settings.Url;
            //do
            //{
            //    jsonData = Uri.EscapeDataString(JsonSerializer.Serialize(data));

            //    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{proc}?data={jsonData}");

            //    var httpResponse = await httpClient.SendAsync(httpRequest);

            //    httpResponse.EnsureSuccessStatusCode();

            //    content = (await httpResponse.Content.ReadAsStringAsync()) ?? string.Empty;

            //    if (content.StartsWith("Error:") == true)
            //        throw new Exception(content.Replace("Error:", "").TrimStart());

            //    data_c["Content"] = content;
            //    await DbHelper.ExecProcAsync("apihole.View_Put", data_c);

            //    data_c["Page"] = data["Page"] = Convert.ToString(data.GetValue("Page", 0) + 1);

            //} while (content.Length == data.GetValue("Size", 0));

            More[result_name] = content;

            await LogHelper.LogInformationAsync(nameof(RunApiHoleAction), new { jobId = JobId, result = "OK", content });

            return true;
        }
    }
}