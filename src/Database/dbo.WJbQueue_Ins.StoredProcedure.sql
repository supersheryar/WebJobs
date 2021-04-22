SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[WJbQueue_Ins]
	@Data nvarchar(max) 
AS
DECLARE @RuleId int = CASE WHEN ISNUMERIC(JSON_VALUE(@Data, '$.Rule')) = 1 THEN JSON_VALUE(@Data, '$.Rule') 
    ELSE (SELECT TOP 1 Id FROM WJbRules WHERE (Name = JSON_VALUE(@Data, '$.Rule'))) END;

INSERT INTO WJbQueue (RuleId, Priority, MoreJson)
SELECT @RuleId, JSON_VALUE(@Data, '$.Priority'), JSON_QUERY(@Data, '$.MoreJson')
--SELECT * FROM OPENJSON(@Data) 
--WITH (RuleId int, Priority tinyint, MoreJson nvarchar(max))

SELECT SCOPE_IDENTITY()
