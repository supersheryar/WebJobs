EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Create_TestJobs]
AS
INSERT INTO WJbQueue ( RuleId, Priority, MoreJson)
SELECT Id, Priority, N''{ "data": "5" }''
FROM WJbRules
WHERE (Id = 2) AND (Disabled = 0)

INSERT INTO WJbQueue ( RuleId, Priority, MoreJson)
SELECT Id, Priority, N''{ "data": "7" }''
FROM WJbRules
WHERE (Id = 2) AND (Disabled = 0)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Delay]
    @Data varchar(10)
AS
DECLARE @Delay DATETIME = DATEADD(SECOND, CAST(@Data AS int), CONVERT(DATETIME, 0))

WAITFOR DELAY @Delay
'
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Finish] 
    @Data varchar(10)
AS
;WITH cte AS (
SELECT TOP 1 Q.Id, Q.[Priority], Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.MoreJson 
    FROM WJbQueue Q
    WHERE Q.Id = CAST(@Data as int) AND Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_FinishAll] 
AS
;WITH cte AS (
SELECT Q.Id, Q.[Priority], Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.MoreJson 
    FROM WJbQueue Q
    WHERE Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Ins]
	@Data nvarchar(max) 
AS
INSERT INTO WJbQueue (RuleId, Priority, MoreJson)
SELECT JSON_VALUE(@Data, ''$.RuleId''), JSON_VALUE(@Data, ''$.Priority''), JSON_QUERY(@Data, ''$.MoreJson'')
--SELECT * FROM OPENJSON(@Data) 
--WITH (RuleId int, Priority tinyint, MoreJson nvarchar(max))

SELECT SCOPE_IDENTITY()
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_InsCron]
AS
INSERT INTO WJbQueue (RuleId, Priority)
SELECT R.Id, R.Priority
FROM WJbRules R
WHERE R.Disabled = 0 
AND NOT JSON_VALUE(MoreJson, ''$.cron'') IS NULL
AND NOT EXISTS (SELECT 1 FROM WJbQueue WHERE RuleId = R.Id)
AND dbo.CronValidate(JSON_VALUE(MoreJson, ''$.cron''), GETDATE()) = 1
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Item]
	@Data varchar(10) 
AS
SELECT TOP (1) Q.*, R.MoreJson RuleMoreJson, A.Name ActionName, A.Type ActionType, A.MoreJson ActionMoreJson
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.Id 
INNER JOIN WJbActions A ON R.ActionId = A.Id
WHERE Q.Id = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start]
    @Data varchar(10)
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE Id = CAST(@Data as int)

EXEC WJbQueue_Item @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start1st]
AS
DECLARE @Job TABLE (Id int);

WITH cte AS (
SELECT TOP 1 Q.Id, Q.Started 
    FROM WJbQueue Q
    INNER JOIN WJbRules R ON Q.RuleId = R.Id
    WHERE Q.Started IS NULL 
    ORDER BY R.Priority ASC, Q.Id ASC)
UPDATE cte 
SET [Started] = GETDATE()
OUTPUT inserted.Id INTO @Job

SELECT TOP (1) Q.*, R.MoreJson RuleMoreJson, A.Name ActionName, A.Type ActionType, A.MoreJson ActionMoreJson
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.Id 
INNER JOIN WJbActions A ON R.ActionId = A.Id
WHERE Q.Id IN (SELECT TOP 1 Id FROM @Job)
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