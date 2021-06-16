SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbQueue_Finish] 
    @Data varchar(100)
AS
;WITH cte AS (
SELECT TOP 1 Q.JobId, Q.JobPriority, Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.JobMore, JSON_VALUE(@Data, '$.JobStatus') JobStatus 
    FROM WJbQueue Q
    WHERE Q.JobId = JSON_VALUE(@Data, '$.JobId') AND Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory
GO
