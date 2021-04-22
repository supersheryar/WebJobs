SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*
EXEC WJbQueue_Start1st
*/
CREATE PROCEDURE [dbo].[WJbQueue_Start1st]
AS
DECLARE @Job TABLE (Id int);

WITH cte AS (
SELECT TOP 1 Q.Id, Q.Started 
    FROM WJbQueue Q
    INNER JOIN WJbRules R ON Q.RuleId = R.Id
    WHERE Q.Started IS NULL 
    ORDER BY R.Priority ASC, Q.Id ASC)
UPDATE cte 
SET [Started] = GETDATE()
OUTPUT inserted.Id INTO @Job

SELECT TOP (1) Q.*, R.MoreJson RuleMoreJson, A.Name ActionName, A.Type ActionType, A.MoreJson ActionMoreJson
FROM WJbQueue Q
INNER JOIN @Job T1 ON Q.Id = T1.Id
INNER JOIN WJbRules R ON Q.RuleId = R.Id 
INNER JOIN WJbActions A ON R.ActionId = A.Id
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER

