SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbRules_Upd_Demo]
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
WHERE RuleId = JSON_VALUE(@Data,'$.RuleId')
GO
