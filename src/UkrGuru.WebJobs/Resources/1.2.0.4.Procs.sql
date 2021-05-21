EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbLogs_Ins]
    @Data nvarchar(max)
AS
INSERT INTO WJbLogs (LogLevel, Title, LogMore)
VALUES (JSON_VALUE(@Data, ''$.logLevel''), JSON_VALUE(@Data, ''$.title''), 
    ISNULL(JSON_QUERY(@Data, ''$.logMore''), JSON_VALUE(@Data, ''$.logMore'')))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Finish] 
    @Data varchar(10)
AS
;WITH cte AS (
SELECT TOP 1 Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore 
    FROM WJbQueue Q
    WHERE Q.JobId = CAST(@Data as int) AND Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_FinishAll] 
AS
;WITH cte AS (
SELECT Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore 
    FROM WJbQueue Q
    WHERE Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Ins]
	@Data nvarchar(max) 
AS
DECLARE @RuleId int = CASE WHEN ISNUMERIC(JSON_VALUE(@Data, ''$.Rule'')) = 1 THEN JSON_VALUE(@Data, ''$.Rule'') 
    ELSE (SELECT TOP 1 RuleId FROM WJbRules WHERE (RuleName = JSON_VALUE(@Data, ''$.Rule''))) END;

INSERT INTO WJbQueue (RuleId, JobPriority, JobMore)
SELECT @RuleId, JSON_VALUE(@Data, ''$.RulePriority''), JSON_QUERY(@Data, ''$.RuleMore'')
--SELECT * FROM OPENJSON(@Data) 
--WITH (RuleId int, RulePriority tinyint, RuleMore nvarchar(max))

SELECT SCOPE_IDENTITY()
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_InsCron]
AS
INSERT INTO WJbQueue (RuleId, JobPriority)
SELECT R.RuleId, R.RulePriority
FROM WJbRules R
WHERE R.Disabled = 0 
AND NOT JSON_VALUE(R.RuleMore, ''$.cron'') IS NULL
AND NOT EXISTS (SELECT 1 FROM WJbQueue WHERE RuleId = R.RuleId)
AND dbo.CronValidate(JSON_VALUE(R.RuleMore, ''$.cron''), GETDATE()) = 1
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Item]
	@Data varchar(10) 
AS
SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
WHERE Q.JobId = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start]
    @Data varchar(10)
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE JobId = CAST(@Data as int)

EXEC WJbQueue_Item @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start1st]
AS
DECLARE @Job TABLE (JobId int);

WITH cte AS (
SELECT TOP 1 Q.JobId, Q.Started 
    FROM WJbQueue Q
    INNER JOIN WJbRules R ON Q.RuleId = R.RuleId
    WHERE Q.Started IS NULL 
    ORDER BY R.RulePriority ASC, Q.JobId ASC)
UPDATE cte 
SET [Started] = GETDATE()
OUTPUT inserted.JobId INTO @Job

SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN @Job T1 ON Q.JobId = T1.JobId
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbSettings_Get]
	@Data nvarchar(1000)
AS
SELECT TOP 1 [Value]
FROM WJbSettings S
WHERE S.Name = JSON_VALUE(@Data, ''$.Name'')
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbSettings_Set]
	@Data nvarchar(1000)
AS
UPDATE dbo.WJbSettings
SET Value = JSON_VALUE(@Data, ''$.Value'')
WHERE (Name = JSON_VALUE(@Data, ''$.Name''))

IF @@ROWCOUNT = 0 BEGIN
    INSERT INTO dbo.WJbSettings (Name, Value)
    VALUES (JSON_VALUE(@Data, ''$.Name''), JSON_VALUE(@Data, ''$.Value''))
END
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Delay_Demo]
    @Data varchar(10)
AS
DECLARE @Delay DATETIME = DATEADD(SECOND, CAST(@Data AS int), CONVERT(DATETIME, 0))
WAITFOR DELAY @Delay
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER   PROCEDURE [dbo].[WJb_Jobs_Ins_Demo]
AS
INSERT INTO WJbQueue (RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "5" }''
FROM WJbRules
WHERE (RuleId = 2) AND (Disabled = 0)

INSERT INTO WJbQueue ( RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "7" }''
FROM WJbRules
WHERE (RuleId = 100) AND (Disabled = 0)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Proc1_Demo]
AS
SELECT CONVERT(varchar(50), GETDATE(), 126)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Proc2_Demo]
    @Data varchar(50)
AS
DECLARE @Start datetime = CAST(@Data as datetime)
SELECT DATEDIFF(SECOND, @Start, GETDATE())
';