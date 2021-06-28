SET IDENTITY_INSERT [dbo].[WJbActions] ON 
INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [Disabled], [ActionType], [ActionMore]) VALUES (1000, N'RunYourSqlProc', 0, N'WebJobsDemo.Actions.YourSqlProcAction, WebJobsDemo', N'{
  "proc": "",
  "data": null,
  "timeout": null,
  "result_name": null
}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF
SET IDENTITY_INSERT [dbo].[WJbRules] ON 
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (1000, N'Your Rule', 0, 2, 1000, N'{
  "proc": "Delay_Demo",
  "data": "7"
}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbActions
SET Disabled = 1
WHERE (ActionId = CAST(@Data AS int))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbActions (ActionName, ActionType, ActionMore)
SELECT * FROM OPENJSON(@Data) 
WITH (ActionName nvarchar(100), ActionType nvarchar(255), ActionMore nvarchar(max))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Item_Demo]
    @Data varchar(10)
AS
SELECT ActionId, ActionName, ActionType, ActionMore
FROM WJbActions
WHERE ActionId = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_List_Demo]
AS
SELECT ActionId, ActionName, ActionType, ActionMore
FROM WJbActions
WHERE (Disabled = 0)
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE A
SET ActionName = D.ActionName
    ,Disabled = 0
    ,ActionType = D.ActionType
    ,ActionMore = D.ActionMore
FROM WJbActions A
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (ActionName nvarchar(100), ActionType nvarchar(255), ActionMore nvarchar(max))) D
WHERE A.ActionId = JSON_VALUE(@Data, ''$.ActionId'')
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_Item_Demo]
    @Data varchar(10)
AS
SELECT H.*, R.RuleName 
FROM WJbHistory H
INNER JOIN WJbRules AS R ON H.RuleId = R.RuleId
WHERE H.JobId = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_List_Demo]
    @Data varchar(100)
AS
DECLARE @Date datetime = CAST(JSON_VALUE(@Data, ''$.Date'') AS date)

SELECT H.*, R.RuleName
FROM (
    SELECT JobId, JobPriority, Created, RuleId, Started, Finished, LEFT(JobMore, 200) JobMore
    FROM WJbHistory
    WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    UNION ALL 
    SELECT JobId, JobPriority, Created, RuleId, Started, Finished, LEFT(JobMore, 200) JobMore
    FROM WJbQueue
    --WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    ) AS H 
INNER JOIN WJbRules AS R ON H.RuleId = R.RuleId
ORDER BY H.JobId ASC
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbRules
SET Disabled = 1
WHERE (RuleId = CAST(@Data AS int))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbRules (RuleName, RulePriority, ActionId, RuleMore)
SELECT * FROM OPENJSON(@Data) 
WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Item_Demo]
    @Data varchar(10)
AS
SELECT R.*, A.ActionName, A.ActionMore
FROM WJbRules AS R 
INNER JOIN WJbActions AS A ON R.ActionId = A.ActionId
WHERE R.RuleId = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_List_Demo]
AS
SELECT R.*, A.ActionName
FROM WJbRules R
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
WHERE R.Disabled = 0 AND A.Disabled = 0
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE R
SET RuleName = D.RuleName
    ,Disabled = 0
    ,RulePriority = D.RulePriority
    ,ActionId = D.ActionId
    ,RuleMore = D.RuleMore
FROM WJbRules R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max))) D
WHERE RuleId = JSON_VALUE(@Data,''$.RuleId'')
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Your_Delay_Demo]
    @Data varchar(10)
AS
DECLARE @Delay DATETIME = DATEADD(SECOND, CAST(@Data AS int), CONVERT(DATETIME, 0))
WAITFOR DELAY @Delay
';