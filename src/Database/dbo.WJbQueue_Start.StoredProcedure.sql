SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbQueue_Start]
    @Data varchar(10)
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE JobId = CAST(@Data as int)

EXEC WJbQueue_Item @Data
GO
