SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbRules_List_Demo]
AS
SELECT R.*, A.Name ActionName
FROM WJbRules R
INNER JOIN WJbActions A ON R.ActionId = A.Id
WHERE R.Disabled = 0 AND A.Disabled = 0
FOR JSON PATH

