# UkrGuru.WebJobs
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.WebJobs)](https://www.nuget.org/packages/UkrGuru.WebJobs/)

UkrGuru.WebJobs package is a scheduler and worker for any of your background jobs under any type of .NET 6 applications. 
Supports cron in the Rules. Provides extensibility for any Custom Actions. Uses UkrGuru.SqlJson for database connection.

Standard Actions:
- DownloadPage - download the page and save the result to WJbFiles table for further processing.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- ProcItems - processing for each(or selected) item of the file.
- RunApiProc - run sql server stored procedure throught ApiHole and stored result in a variable for further processing.
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.

SSRS Actions:
- ExportReport - export SSRS report to WJbFiles table.

SshNet Actions:
- GetFiles - download files from remote directory to WJbFiles table.
- PutFiles - upload files from WJbFiles to remote directory.

MailKit Actions:
- ReceiveEmails - receive emails from the mailbox in WJbFiles and WJbQueue for further processing.

CsvHelper Actions:
- ImportFile - import CSV file into WJbItems table.