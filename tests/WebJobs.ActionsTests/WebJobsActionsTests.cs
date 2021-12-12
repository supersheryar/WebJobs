using Xunit;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using UkrGuru.WebJobs.Actions;

namespace System.Reflection.Tests
{
    public class WebJobsActionsTests
    {
        private readonly bool dbOK;

        public WebJobsActionsTests()
        {
            var dbName = "WebJobsTest";

            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

            var dbInitScript = $"IF DB_ID('{dbName}') IS NOT NULL BEGIN " +
                $"  ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                $"  DROP DATABASE {dbName}; " +
                $"END " +
                $"CREATE DATABASE {dbName};";

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
            DbHelper.ExecCommand(dbInitScript);

            DbHelper.ConnectionString = connectionString;
            dbOK = Assembly.GetAssembly(typeof(BaseAction)).InitDb();
        }

        [Fact]
        public void InitDbTest()
        {
            Assert.True(dbOK);
        }

        [Fact]
        public void WJbSettingsTests()
        {
            DbHelper.ExecProc("WJbSettings_Set", new { Name = "Name1", Value = "Value1" });

            Assert.Equal("Value1", DbHelper.FromProc("WJbSettings_Get", "Name1"));

            DbHelper.ExecProc("WJbSettings_Set", new { Name = "Name1", Value = "Value11" });

            Assert.Equal("Value11", DbHelper.FromProc("WJbSettings_Get", "Name1"));
        }

        [Fact]
        public void WJbFilesTests()
        {
            var content = new byte[256]; for (int i = 0; i < 256; i++) content[i] = (byte)i;

            var guid = DbHelper.FromProc("WJbFiles_Ins", new File { FileName = "test.bin", FileContent = content });

            var file = DbHelper.FromProc<File>("WJbFiles_Get", guid);

            Assert.Equal(file.FileContent, content);
        }

        [Fact]
        public void WJbLogsTests()
        {
            DbHelper.ExecProc("WJbLogs_Ins", new { logLevel = LogLevel.Information, title="Test #1", logMore = "Test #1" });

            DbHelper.ExecProc("WJbLogs_Ins", new { logLevel = LogLevel.Information, title = "Test #2", logMore = new { jobId = 2, result = "OK" } });

            Assert.True(true);
        }

        [Fact]
        public void BaseActionTest()
        {
            Job job = new() { ActionType = "BaseAction, UkrGuru.WebJobs.Actions" };

            //job.JobMore = @"{ ""next"": ""Rule"", ""data"": """", ""enabled"": true }"
            //"result_name": "next_data", "next": "1", "next_proc": "PlannedJobs_Proc" }

            var action = job.CreateAction(); 

            var result = action.ExecuteAsync().Result;

            action.NextAsync(result).Wait();

            Assert.True(true);
        }

        [Fact]
        public void SqlProcActionTest()
        {
            DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[WJb_HelloTest] (@Data varchar(100)) AS SELECT 'Hello ' + @Data  + '!'");

            Job job = new() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs.Actions" };
            job.JobMore = @"{ ""proc"": ""HelloTest"", ""data"": ""Alex"", ""result_name"": ""proc_result"" }";

            var action = job.CreateAction();

            var result = action.ExecuteAsync().Result;
            // action.NextAsync(result).Wait();

            var proc_result = ((More)action.More).GetValue("proc_result");

            Assert.Equal("Hello Alex!", proc_result);
        }

        //[Fact]
        //public void SendEmailActionTest()
        //{
        //    Assert.True(true);
        //}

        [Fact]
        public void FillTemplateActionTest()
        {
            Job job = new() { ActionType = "FillTemplateAction, UkrGuru.WebJobs.Actions" };

            job.ActionMore = @"{ ""tname_pattern"": ""[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"" }";
            job.JobMore = @"{ ""template_subject"": ""Hello DEAR_NAME!"", ""tvalue_DEAR_NAME"": ""Alex"" }";

            var action = job.CreateAction();

            var result = action.ExecuteAsync().Result;
            // action.NextAsync(result).Wait();

            var subject = ((More)action.More).GetValue("next_subject");

            Assert.Equal("Hello Alex!", subject);
        }

        //[Fact]
        //public void Url2HtmlActionTest()
        //{
        //    Job job = new() { ActionType = "Url2HtmlAction, UkrGuru.WebJobs.Actions" };
        //    job.JobMore = @"{ ""url"": ""https://ukrguru.com/"", ""result_name"": ""next_body"" }";

        //    var action = job.CreateAction();

        //    var result = action.ExecuteAsync().Result;
        //    // action.NextAsync(result).Wait();

        //    var guid = ((More)action.More).GetValue("next_body");

        //    var file = DbHelper.FromProc<File>("WJbFiles_Get", guid);

        //    var body = Text.Encoding.UTF8.GetString(file.FileContent, 0, file.FileContent.Length);

        //    Assert.Contains("Oleksandr Viktor (UkrGuru)", body);
        //}
    }
}
