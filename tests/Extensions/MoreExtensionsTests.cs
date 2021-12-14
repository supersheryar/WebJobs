using System;
using Xunit;

namespace UkrGuru.WebJobs.Data.Tests
{
    public class MoreExtensionsTests
    {
        [Fact]
        public void AddNewTest()
        {
            Assert.Equal("more", Assert.Throws<ArgumentNullException>(() => ((More)null).AddNew(null)).ParamName);

            More more = new();
            more.AddNew(null);
            more.AddNew("");
            more.AddNew(" ");
            more.AddNew(@"{ ""type"": ""Rule"", ""data"": """", ""enabled"": true }");
            more.AddNew(@"{ ""type"": ""Action"", ""timeout"": 60, ""amount"": 123.45 }");

            Assert.Equal("Rule", more.GetValue("type"));
            Assert.Empty(more.GetValue("data"));
            Assert.Equal(60, more.GetValue("timeout", 0));
            Assert.Equal(true, more.GetValue("enabled", false));
        }
    }
}