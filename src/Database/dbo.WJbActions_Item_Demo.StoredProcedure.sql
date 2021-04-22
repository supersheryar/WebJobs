SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbActions_Item_Demo]
    @Data varchar(10)
AS
SELECT Id, Name, Type, MoreJson
FROM WJbActions
WHERE Id = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER

