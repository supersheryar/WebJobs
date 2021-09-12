# UkrGuru.WebJobs

UkrGuru.WebJobs package is a scheduler and worker for any of your background jobs under any type of .NET 5 applications. 
Supports cron in the Rules. Provides extensibility for any Custom Actions.

Available Standard Actions:
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- Url2Html - load the page from the specified url and stored result in a variable for further processing.

Available SSH.NET Actions:
- SshNetGetFiles - downloads files from remote directory to local directory.
- SshNetPutFiles - uploads files from local directory to remote directory.

![Demo_App](https://github.com/UkrGuru/WebJobs/blob/main/docs/images/webjobs-demo.gif)

## Documentation
- ![Getting Started Guide](#)
- ![How-to Guide](#)

## Demos
- ![WebJobsDemo](#https://github.com/UkrGuru/WebJobs/tree/main/demos/WebJobsDemo) 
- ![WebJobsService](#https://github.com/UkrGuru/WebJobs/tree/main/demos/WebJobsService) 
- ![CustomActions](#https://github.com/UkrGuru/WebJobs/tree/main/demos/CustomActions) 

## Installation

### Prerequisites

- IDE - Visual Studio 2019 or higher
- Framework - .NET 5 or higher
- Database - SQL Server 2019 Express or higher 

### Clone

- Clone this repo to your local machine using `https://github.com/UkrGuru/WebJobs.git`

### Setup

> WebJobsDemo
- Create a new database `WebJobsDemo` in SQL Server
- Make sure the `SqlJsonConnection` is correctly configured for this database in appsettings.json

> WebJobsService
- Create a new database `WebJobsService` in SQL Server
- Make sure the `SqlJsonConnection` is correctly configured for this database in appsettings.json

> WebJobsApi
- Create a new database `WebJobsApi` in SQL Server
- Make sure the `SqlJsonConnection` is correctly configured for this database in appsettings.json
- Enter new your secret values for WJaSettings in appsettings.json