# UkrGuru.WebJobs

UkrGuru.WebJobs is a Starter Library for running background tasks under your ASP.NET Core website with Sql Server.
Supports cron and trigger Rules. Allows extensibility for any custom Action.

## Installation

### Prerequisites

- IDE - Visual Studio 2019 or higher
- Framework - .NET Core 3.0 or higher
- Database - SQL Server 2016 Express or higher 

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
