SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbRules_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbRules
SET Disabled = 1
WHERE (RuleId = CAST(@Data AS int))
GO
