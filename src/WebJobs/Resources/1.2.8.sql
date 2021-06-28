EXEC dbo.sp_executesql @statement = N'
UPDATE WJbActions
SET ActionType = ''SqlProcAction, UkrGuru.WebJobs.Actions''
WHERE ActionId = 1
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJb_Jobs_Ins_Demo]
AS
INSERT INTO WJbQueue (RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "3" }''
FROM WJbRules
WHERE (RuleId = 2) AND (Disabled = 0)

INSERT INTO WJbQueue ( RuleId, JobPriority, JobMore)
SELECT RuleId, RulePriority, N''{ "data": "5" }''
FROM WJbRules
WHERE (RuleId = 2) AND (Disabled = 0)

INSERT INTO WJbQueue ( RuleId, JobPriority)
SELECT RuleId, RulePriority
FROM WJbRules
WHERE (RuleId = 1000) AND (Disabled = 0)
';
