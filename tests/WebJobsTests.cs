using Xunit;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using UkrGuru.WebJobs.Actions;
using System.Threading.Tasks;

namespace System.Reflection.Tests
{
    public class WebJobsTests
    {
        private readonly bool dbOK;

        public WebJobsTests()
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

            var assembly = Assembly.GetAssembly(typeof(BaseAction));
            ArgumentNullException.ThrowIfNull(assembly);

            dbOK = assembly.InitDb();
            //dbOK = true;
        }

        [Fact]
        public void InitDbTest()
        {
            Assert.True(dbOK);
        }

        [Fact]
        public async Task WJbSettingsTests()
        {
            await DbHelper.ExecProcAsync("WJbSettings_Set", new { Name = "Name1", Value = "Value1" });

            Assert.Equal("Value1", DbHelper.FromProc("WJbSettings_Get", "Name1"));

            await DbHelper.ExecProcAsync("WJbSettings_Set", new { Name = "Name1", Value = "Value11" });

            Assert.Equal("Value11", DbHelper.FromProc("WJbSettings_Get", "Name1"));
        }

        [Fact]
        public async Task WJbFilesTests()
        {
            var content = new byte[256]; for (int i = 0; i < 256; i++) content[i] = (byte)i;

            var guid = await DbHelper.FromProcAsync("WJbFiles_Ins", new File { FileName = "test.bin", FileContent = content });

            var file = await DbHelper.FromProcAsync<File>("WJbFiles_Get", guid);

            Assert.Equal(file?.FileContent, content);
        }

        [Fact]
        public async Task WJbLogsTests()
        {
            await DbHelper.ExecProcAsync("WJbLogs_Ins", new { logLevel = LogLevel.Information, title = "Test #1", logMore = "Test #1" });

            await DbHelper.ExecProcAsync("WJbLogs_Ins", new { logLevel = LogLevel.Information, title = "Test #2", logMore = new { jobId = 2, result = "OK" } });

            Assert.True(true);
        }

        [Fact]
        public async Task BaseActionTest()
        {
            Job job = new() { ActionType = "BaseAction, UkrGuru.WebJobs" };

            //job.JobMore = @"{ ""next"": ""Rule"", ""data"": """", ""enabled"": true }"
            //"result_name": "next_data", "next": "1", "next_proc": "PlannedJobs_Proc" }

            bool result = false;

            var action = job.CreateAction();

            if (action != null)
            {
                result = await action.ExecuteAsync();

                await action.NextAsync(result);
            }

            Assert.True(result);
        }

        [Fact]
        public async Task RunSqlProcActionTest()
        {
            await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[WJb_HelloTest] (@Data varchar(100)) AS SELECT 'Hello ' + @Data  + '!'");

            Job job = new() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs" };
            job.JobMore = @"{ ""proc"": ""HelloTest"", ""data"": ""Alex"", ""result_name"": ""proc_result"" }";


            var proc_result = null as string;

            var action = job.CreateAction();

            if (action != null)
            {
                await action.ExecuteAsync();

                proc_result = ((More)action.More).GetValue("proc_result");
            }

            Assert.Equal("Hello Alex!", proc_result);
        }

        //[Fact]
        //public void SendEmailActionTest()
        //{
        //    Assert.True(true);
        //}

        [Fact]
        public async Task FillTemplateActionTest()
        {
            Job job = new() { ActionType = "FillTemplateAction, UkrGuru.WebJobs" };

            job.ActionMore = @"{ ""tname_pattern"": ""[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"" }";
            job.JobMore = @"{ ""template_subject"": ""Hello DEAR_NAME!"", ""tvalue_DEAR_NAME"": ""Alex"" }";

            var subject = null as string;

            var action = job.CreateAction();

            if (action != null)
            {
                await action.ExecuteAsync();
                subject = ((More)action.More).GetValue("next_subject");
            }

            Assert.Equal("Hello Alex!", subject);
        }

        //[Fact]
        //public async Task RunApiProcActionTest()
        //{
        //    //await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[WJb_HelloTest] (@Data varchar(100)) AS SELECT 'Hello ' + @Data  + '!'");

        //    Job job = new() {  ActionType = "RunApiProcAction, UkrGuru.WebJobs" };
        //    job.RuleMore = @"{  ""api_settings_name"": ""WDogApi"" }";
        //    job.JobMore = @"{ ""proc"": ""HelloTest"", ""data"": ""Alex"", ""result_name"": ""proc_result"" }";

        //    var action = job.CreateAction();

        //    var result = await action.ExecuteAsync();
        //    // action.NextAsync(result).Wait();

        //    var proc_result = ((More)action.More).GetValue("proc_result");

        //    Assert.Equal("Hello Alex!", proc_result);
        //}

        //[Fact]
        //public void Url2HtmlActionTest()
        //{
        //    Job job = new() { ActionType = "Url2HtmlAction, UkrGuru.WebJobs" };
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
