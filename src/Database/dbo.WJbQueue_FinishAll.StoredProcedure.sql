SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbQueue_FinishAll] 
AS
;WITH cte AS (
SELECT Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore, 5 /* Cancelled */ JobStatus 
    FROM WJbQueue Q
    WHERE Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
GO
