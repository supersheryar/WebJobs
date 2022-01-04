# UkrGuru.WebJobs

UkrGuru.WebJobs package is a scheduler and worker for any of your background jobs under any type of .NET 5 applications. 
Supports cron in the Rules. Provides extensibility for any Custom Actions. Uses UkrGuru.SqlJson for database connection.

Available Standard Actions:
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- RunApiProc - run sql server stored procedure throught ApiHole and stored result in a variable for further processing.
