SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*
EXEC WJbQueue_Start1st
*/
CREATE PROCEDURE [dbo].[WJbQueue_Start]
    @Data varchar(10)
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE Id = CAST(@Data as int)

EXEC WJbQueue_Item @Data

