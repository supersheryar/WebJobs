using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace UkrGuru.WebJobs;

public class Utility
{
    public static bool IsEmailAddress(string address) => address != null && new EmailAddressAttribute().IsValid(address);

    public static bool IsHtmlBody(string? body) => body != null && Regex.IsMatch(body, @"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");  // or @"<[^>]+>"
}
