SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbActions_Item_Demo]
    @Data varchar(10)
AS
SELECT ActionId, ActionName, ActionType, ActionMore
FROM WJbActions
WHERE ActionId = CAST(@Data AS int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
