using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using Xunit;
using File = System.IO.File;

namespace SshNetTests;

public class SshNetTests
{
    private readonly bool dbOK = false;

    public SshNetTests()
    {
        var dbName = "SshNetTest";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        //var dbInitScript = $"IF DB_ID('{dbName}') IS NOT NULL BEGIN " +
        //    $"  ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
        //    $"  DROP DATABASE {dbName}; " +
        //    $"END " +
        //    $"CREATE DATABASE {dbName};";

        //DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
        //DbHelper.ExecCommand(dbInitScript);

        DbHelper.ConnectionString = connectionString;

        if (dbOK) return;
        
        var assembly1 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
        ArgumentNullException.ThrowIfNull(assembly1);
        dbOK = assembly1.InitDb();

        var assembly2 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.SshNet.GetFilesAction));
        ArgumentNullException.ThrowIfNull(assembly2);
        dbOK &= assembly2.InitDb();

        var assembly3 = Assembly.GetAssembly(typeof(SshNetTests));
        ArgumentNullException.ThrowIfNull(assembly3);
        dbOK &= assembly3.InitDb();

        //dbOK = true;
    }

    [Fact]
    public void InitDbTest()
    {
        Assert.True(dbOK);
    }

    [Fact]
    public void SshNetTest()
    {
        string filename = @"test\\1.txt";

        if (!Directory.Exists("test")) Directory.CreateDirectory("Test");

        File.WriteAllText(filename, new String('1', 4096));

        Assert.True(File.Exists(filename));

        TestRule(101);

        Assert.False(File.Exists(filename));

        TestRule(100);

        Assert.True(File.Exists(filename));

        var text = File.ReadAllText(filename);

        Assert.Equal(text, new String('1', 4096));
    }

    static void TestRule(int ruleId)
    {
        var jobId = DbHelper.FromProc<int>("WJbRules_Test", ruleId);

        TestJob(jobId);
    }

    static void TestJob(int jobId)
    {
        var job = DbHelper.FromProc<JobQueue>("WJbQueue_Start", jobId.ToString());

        if (job?.JobId > 0)
        {
            bool result = false;
            try
            {
                var action = job.CreateAction();

                if (action != null)
                {
                    result = action.ExecuteAsync().Result;

                    // action.NextAsync(result).Wait();
                }
            }
            catch
            {
                result = false;
                throw;
            }
            finally
            {
                DbHelper.ExecProcAsync("WJbQueue_Finish", new { JobId = jobId, JobStatus = result ? JobStatus.Completed : JobStatus.Failed }).Wait();
            }
        }
    }
}