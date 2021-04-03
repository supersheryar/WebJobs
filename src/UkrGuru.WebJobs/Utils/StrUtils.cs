namespace UkrGuru.WebJobs.Utils
{
    public class StrUtils
    {
        public static string ShortStr(string text, int maxLength) => (!string.IsNullOrEmpty(text) && text.Length > maxLength) ? text.Substring(0, maxLength) + "..." : text;
    }
}
