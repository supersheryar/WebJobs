EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Ins]
    @Data nvarchar(max)
AS
DECLARE @Id uniqueidentifier = NEWID();

INSERT WJbFiles (Id, Created, FileName, FileContent)
SELECT @Id Id, GETDATE() Created, * 
FROM OPENJSON(@Data) 
WITH (FileName nvarchar(100), FileContent varbinary(max))

SELECT CAST(@Id as varchar(50)) Id
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Get]
	@Data uniqueidentifier
AS
SELECT Id, Created, FileName, FileContent
FROM WJbFiles
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbLogs_Ins]
    @Data nvarchar(max)
AS
INSERT INTO WJbLogs (LogLevel, Title, LogMore)
VALUES (JSON_VALUE(@Data, ''$.logLevel''), JSON_VALUE(@Data, ''$.title''), 
    ISNULL(JSON_QUERY(@Data, ''$.logMore''), JSON_VALUE(@Data, ''$.logMore'')))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_Finish] 
    @Data varchar(100)
AS
WITH cte 
AS (
    SELECT TOP 1 JobId, JobPriority, Created, RuleId, Started, GETDATE() AS Finished, JobMore, JSON_VALUE(@Data, ''$.JobStatus'') JobStatus 
    FROM WJbQueue
    WHERE JobId = JSON_VALUE(@Data, ''$.JobId'') AND Started IS NOT NULL
    )
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_FinishAll] 
AS
;WITH cte 
AS (
    SELECT JobId, JobPriority, Created, RuleId, Started, GETDATE() AS Finished, JobMore, 5 /* Cancelled */ JobStatus 
    FROM WJbQueue
    WHERE Started IS NOT NULL
    )
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_Ins]
	@Data nvarchar(max) 
AS
DECLARE @RuleId int = CASE WHEN ISNUMERIC(JSON_VALUE(@Data, ''$.Rule'')) = 1 THEN JSON_VALUE(@Data, ''$.Rule'') 
    ELSE (SELECT TOP 1 RuleId FROM WJbRules WHERE (RuleName = JSON_VALUE(@Data, ''$.Rule''))) END;

INSERT INTO WJbQueue (RuleId, JobPriority, JobMore, JobStatus)
SELECT @RuleId, JSON_VALUE(@Data, ''$.RulePriority''), JSON_QUERY(@Data, ''$.RuleMore''), 1 /* Queued */

SELECT CAST(SCOPE_IDENTITY() AS varchar) Id
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_InsCron]
AS
INSERT INTO WJbQueue (RuleId, JobPriority, JobStatus)
SELECT R.RuleId, R.RulePriority, 1 /* Queued */ 
FROM WJbRules R
WHERE R.Disabled = 0 
AND NOT JSON_VALUE(R.RuleMore, ''$.cron'') IS NULL
AND NOT EXISTS (SELECT 1 FROM WJbQueue WHERE RuleId = R.RuleId)
AND dbo.CronValidate(JSON_VALUE(R.RuleMore, ''$.cron''), GETDATE()) = 1
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_Get]
	@Data int
AS
SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
WHERE Q.JobId = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbQueue_Start1st]
AS
DECLARE @JobId int;

;WITH cte 
AS (
    SELECT TOP 1 JobId, Started, JobStatus 
    FROM WJbQueue
    WHERE [Started] IS NULL 
    ORDER BY JobPriority ASC, JobId ASC
    )
UPDATE cte 
SET @JobId = JobId, [Started] = GETDATE(), JobStatus = 2 /* Running */

EXEC WJbQueue_Get @JobId
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbSettings_Get]
	@Data nvarchar(100)
AS
SELECT TOP 1 [Value]
FROM WJbSettings
WHERE Name = @Data
';
EXEC dbo.sp_executesql @statement = N'
/*
EXEC WJbSettings_Set ''{ "Name":"Name1", "Value":"Value1" }''
*/
CREATE OR ALTER PROCEDURE [WJbSettings_Set]
	@Data nvarchar(max)
AS
DECLARE @Name nvarchar(100), @Value nvarchar(max)

SELECT @Name = D.[Name], @Value = D.[Value]
FROM OPENJSON(@Data) WITH ([Name] nvarchar(100), [Value] nvarchar(max)) D

UPDATE WJbSettings
SET [Value] = @Value
WHERE ([Name] = @Name)

IF @@ROWCOUNT = 0 
    INSERT INTO WJbSettings ([Name], [Value]) 
    VALUES (@Name, @Value)
';