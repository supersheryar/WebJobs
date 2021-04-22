SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*
DECLARE @Data varchar(50) = CONVERT(varchar(50), GETDATE(), 126)
EXEC [dbo].[WJb_Proc2] @Data
*/
CREATE PROCEDURE [dbo].[WJb_Proc2_Demo]
    @Data varchar(50)
AS
DECLARE @Start datetime = CAST(@Data as datetime)
SELECT DATEDIFF(SECOND, @Start, GETDATE())

