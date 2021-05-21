SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbQueue_Item]
	@Data varchar(10) 
AS
SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
WHERE Q.JobId = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
