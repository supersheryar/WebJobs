SET IDENTITY_INSERT [dbo].[WJbActions] ON 

INSERT [dbo].[WJbActions] ([Id], [Name], [Disabled], [Type], [MoreJson]) VALUES (100, N'RunYourSqlProc', 0, N'WebJobsDemo.Actions.YourSqlProcAction, WebJobsDemo', N'{
  "proc": null,
  "data": null
}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF
SET IDENTITY_INSERT [dbo].[WJbRules] ON 

INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (100, N'Your Rule', 0, 2, 100, N'{
  "proc": "Delay"
}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
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
WHERE (Id = 100) AND (Disabled = 0)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbActions
SET Disabled = 1
WHERE (Id = CAST(@Data AS int))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbActions (Name, Type, MoreJson)
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(100), Type nvarchar(255), MoreJson nvarchar(max))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Item_Demo]
    @Data varchar(10)
AS
SELECT Id, Name, Type, MoreJson
FROM WJbActions
WHERE Id = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_List_Demo]
AS
SELECT Id, Name, Type, MoreJson
FROM WJbActions
WHERE (Disabled = 0)
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE A
SET Name = D.Name
    ,Disabled = 0
    ,Type = D.Type
    ,MoreJson = D.MoreJson
FROM WJbActions A
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (Name nvarchar(100), Type nvarchar(255), MoreJson nvarchar(max))) D
WHERE A.Id = JSON_VALUE(@Data, ''$.Id'')
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_Item_Demo]
    @Data varchar(10)
AS
SELECT H.*, R.Name RuleName 
FROM WJbHistory H
INNER JOIN WJbRules AS R ON H.RuleId = R.Id
WHERE H.Id = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_List_Demo]
    @Data varchar(100)
AS
DECLARE @Date datetime = CAST(JSON_VALUE(@Data, ''$.Date'') AS date)

SELECT H.*, R.Name RuleName
FROM (
    SELECT Id, Priority, Created, RuleId, Started, Finished, LEFT(MoreJson, 200) MoreJson
    FROM WJbHistory
    WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    UNION ALL 
    SELECT Id, Priority, Created, RuleId, Started, Finished, LEFT(MoreJson, 200) MoreJson
    FROM WJbQueue
    --WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    ) AS H 
INNER JOIN WJbRules AS R ON H.RuleId = R.Id
ORDER BY H.Id ASC
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbRules
SET Disabled = 1
WHERE (Id = CAST(@Data AS int))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbRules (Name, Priority, ActionId, MoreJson)
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(100), Priority tinyint, ActionId int, MoreJson nvarchar(max))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Item_Demo]
    @Data varchar(10)
AS
SELECT R.*, A.Name ActionName, A.MoreJson ActionMoreJson
FROM WJbRules AS R 
INNER JOIN WJbActions AS A ON R.ActionId = A.Id
WHERE R.Id = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_List_Demo]
AS
SELECT R.*, A.Name ActionName
FROM WJbRules R
INNER JOIN WJbActions A ON R.ActionId = A.Id
WHERE R.Disabled = 0 AND A.Disabled = 0
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE R
SET Name = D.Name
    ,Disabled = 0
    ,Priority = D.Priority
    ,ActionId = D.ActionId
    ,MoreJson = D.MoreJson
FROM WJbRules R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (Name nvarchar(100), Priority tinyint, ActionId int, MoreJson nvarchar(max))) D
WHERE Id = JSON_VALUE(@Data,''$.Id'')
';
