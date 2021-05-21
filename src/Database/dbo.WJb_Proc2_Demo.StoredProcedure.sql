SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJb_Proc2_Demo]
    @Data varchar(50)
AS
DECLARE @Start datetime = CAST(@Data as datetime)
SELECT DATEDIFF(SECOND, @Start, GETDATE())
GO
