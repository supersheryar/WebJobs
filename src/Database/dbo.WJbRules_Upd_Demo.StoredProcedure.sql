SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbRules_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE R
SET RuleName = D.RuleName
    ,RulePriority = D.RulePriority
    ,ActionId = D.ActionId
    ,RuleMore = D.RuleMore
    ,Disabled = D.[Disabled]
FROM WJbRules R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max), [Disabled] bit)) D
WHERE RuleId = JSON_VALUE(@Data,'$.RuleId')
GO
