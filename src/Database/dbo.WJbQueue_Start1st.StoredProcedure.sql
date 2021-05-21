SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbQueue_Start1st]
AS
DECLARE @Job TABLE (JobId int);

WITH cte AS (
SELECT TOP 1 Q.JobId, Q.Started 
    FROM WJbQueue Q
    INNER JOIN WJbRules R ON Q.RuleId = R.RuleId
    WHERE Q.Started IS NULL 
    ORDER BY R.RulePriority ASC, Q.JobId ASC)
UPDATE cte 
SET [Started] = GETDATE()
OUTPUT inserted.JobId INTO @Job

SELECT TOP (1) Q.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
FROM WJbQueue Q
INNER JOIN @Job T1 ON Q.JobId = T1.JobId
INNER JOIN WJbRules R ON Q.RuleId = R.RuleId 
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
