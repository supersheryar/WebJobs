SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbHistory_Item_Demo]
    @Data varchar(10)
AS
SELECT H.*, R.RuleName 
FROM WJbHistory H
INNER JOIN WJbRules AS R ON H.RuleId = R.RuleId
WHERE H.JobId = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
