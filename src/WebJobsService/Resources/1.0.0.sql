SET IDENTITY_INSERT [dbo].[WJbActions] ON 
INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [Disabled], [ActionType], [ActionMore]) VALUES (1001, N'RunYourSqlProc', 0, N'WebJobsService.Actions.YourSqlProcAction, WebJobsService', N'{
  "proc": "",
  "data": null,
  "timeout": null,
  "result_name": null
}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF
SET IDENTITY_INSERT [dbo].[WJbRules] ON 
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (1001, N'Your Rule', 0, 2, 1001, N'{
  "proc": "Delay_Demo",
  "data": "7"
}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Your_Delay_Demo]
    @Data varchar(10)
AS
DECLARE @Delay DATETIME = DATEADD(SECOND, CAST(@Data AS int), CONVERT(DATETIME, 0))
WAITFOR DELAY @Delay
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Jobs_Ins_Demo]
AS
INSERT INTO WJbQueue (RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "3" }''
FROM WJbRules
WHERE (RuleId = 2) AND (Disabled = 0)

INSERT INTO WJbQueue (RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "5" }''
FROM WJbRules
WHERE (RuleId = 2) AND (Disabled = 0)

INSERT INTO WJbQueue (RuleId, JobPriority)
SELECT RuleId, RulePriority 
FROM WJbRules
WHERE (RuleId = 1001) AND (Disabled = 0)
';
