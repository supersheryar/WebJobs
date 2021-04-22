SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbActions_Upd_Demo]
	@Data nvarchar(max) 
AS
UPDATE A
SET Name = D.Name
    ,Disabled = 0
    ,Type = D.Type
    ,MoreJson = D.MoreJson
FROM WJbActions A
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (Name nvarchar(100), Type nvarchar(255), MoreJson nvarchar(max))) D
WHERE A.Id = JSON_VALUE(@Data, '$.Id')

