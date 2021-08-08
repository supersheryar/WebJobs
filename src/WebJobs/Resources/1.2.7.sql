EXEC dbo.sp_executesql @statement = N'
IF OBJECT_ID(''dbo.DF_WJbQueue_JobStatus'', ''D'') IS NULL 
    ALTER TABLE dbo.WJbQueue ADD JobStatus tinyint NOT NULL CONSTRAINT DF_WJbQueue_JobStatus DEFAULT 0;
IF OBJECT_ID(''dbo.DF_WJbHistory_JobStatus'', ''D'') IS NULL 
    ALTER TABLE dbo.WJbHistory ADD JobStatus tinyint NOT NULL CONSTRAINT DF_WJbHistory_JobStatus DEFAULT 0;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Ins]
	@Data nvarchar(max) 
AS
DECLARE @RuleId int = CASE WHEN ISNUMERIC(JSON_VALUE(@Data, ''$.Rule'')) = 1 THEN JSON_VALUE(@Data, ''$.Rule'') 
    ELSE (SELECT TOP 1 RuleId FROM WJbRules WHERE (RuleName = JSON_VALUE(@Data, ''$.Rule''))) END;

INSERT INTO WJbQueue (RuleId, JobPriority, JobMore, JobStatus)
SELECT @RuleId, JSON_VALUE(@Data, ''$.RulePriority''), JSON_QUERY(@Data, ''$.RuleMore''), 1 /* Queued */
--SELECT * FROM OPENJSON(@Data) 
--WITH (RuleId int, RulePriority tinyint, RuleMore nvarchar(max))

SELECT SCOPE_IDENTITY()
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_InsCron]
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
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start]
    @Data varchar(10)
AS
UPDATE WJbQueue
SET Started = GETDATE(), JobStatus = 2 /* Running */
WHERE JobId = CAST(@Data as int)

EXEC WJbQueue_Item @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start1st]
AS
DECLARE @Job TABLE (JobId int);

WITH cte AS (
SELECT TOP 1 Q.JobId, Q.Started, Q.JobStatus 
    FROM WJbQueue Q
    INNER JOIN WJbRules R ON Q.RuleId = R.RuleId
    WHERE Q.Started IS NULL 
    ORDER BY R.RulePriority ASC, Q.JobId ASC)
UPDATE cte 
SET [Started] = GETDATE(), JobStatus = 2 /* Running */
OUTPUT inserted.JobId INTO @Job

SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN @Job T1 ON Q.JobId = T1.JobId
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Finish] 
    @Data varchar(100)
AS
;WITH cte AS (
SELECT TOP 1 Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore, JSON_VALUE(@Data, ''$.JobStatus'') JobStatus 
    FROM WJbQueue Q
    WHERE Q.JobId = JSON_VALUE(@Data, ''$.JobId'') AND Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_FinishAll] 
AS
;WITH cte AS (
SELECT Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore, 5 /* Cancelled */ JobStatus 
    FROM WJbQueue Q
    WHERE Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
