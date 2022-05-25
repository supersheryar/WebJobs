// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

InitWebJobsDb("MyWebJobsTest");

// Add Your Custom Action and Rule
Assembly.GetExecutingAssembly().InitDb();

try
{
    TestRule(1000);
    Console.WriteLine("Success!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    Console.ReadKey();
}

static bool InitWebJobsDb(string dbName)
{
    var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

    DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

    DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

    DbHelper.ConnectionString = connectionString;

    var assembly = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
    ArgumentNullException.ThrowIfNull(assembly);

    return assembly.InitDb();
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