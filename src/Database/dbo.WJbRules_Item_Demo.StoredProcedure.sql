SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbRules_Item_Demo]
    @Data varchar(10)
AS
SELECT R.*, A.Name ActionName, A.MoreJson ActionMoreJson
FROM WJbRules AS R 
INNER JOIN WJbActions AS A ON R.ActionId = A.Id
WHERE R.Id = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER

