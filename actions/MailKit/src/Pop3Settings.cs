using System.Text.Json.Serialization;

namespace UkrGuru.WebJobs.Actions.MailKit;

public class Pop3Settings
{
    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("useSsl")]
    public bool UseSsl { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}