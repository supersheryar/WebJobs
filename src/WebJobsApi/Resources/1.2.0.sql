EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJa_Actions_Del_Demo]
    @Data varchar(10)
AS
UPDATE WJbActions
SET Disabled = 1
WHERE (ActionId = CAST(@Data AS int))
';
