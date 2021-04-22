SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbRules_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE R
SET Name = D.Name
    ,Disabled = 0
    ,Priority = D.Priority
    ,ActionId = D.ActionId
    ,MoreJson = D.MoreJson
FROM WJbRules R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (Name nvarchar(100), Priority tinyint, ActionId int, MoreJson nvarchar(max))) D
WHERE Id = JSON_VALUE(@Data,'$.Id')

