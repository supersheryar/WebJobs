SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbRules_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbRules
SET Disabled = 1
WHERE (Id = CAST(@Data AS int))

