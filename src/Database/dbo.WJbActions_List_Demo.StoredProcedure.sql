SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbActions_List_Demo]
AS
SELECT Id, Name, Type, MoreJson
FROM WJbActions
WHERE (Disabled = 0)
FOR JSON PATH

