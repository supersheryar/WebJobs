SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbRules_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbRules (RuleName, RulePriority, ActionId, RuleMore)
SELECT * FROM OPENJSON(@Data) 
WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max))
GO
