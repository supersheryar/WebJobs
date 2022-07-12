# UkrGuru.WebJobs

UkrGuru.WebJobs package is a scheduler and worker for any of your background jobs under any type of .NET 6 applications. 
Supports cron in the Rules. Provides extensibility for any Custom Actions. Uses UkrGuru.SqlJson for database connection.

Standard Actions:
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- DownloadPage - download the page and save the result to a WJbFiles table for further processing.
- RunApiProc - run sql server stored procedure throught ApiHole and stored result in a variable for further processing.

SSRS Actions:
- ExportReport - export SSRS report to a WJbFiles table for further processing.

SshNet Actions:
- GetFiles - download files from remote directory to local directory.
- PutFiles - upload files from local directory to remote directory.
