SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[WJbActions_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbActions (ActionName, ActionType, ActionMore)
SELECT * FROM OPENJSON(@Data) 
WITH (ActionName nvarchar(100), ActionType nvarchar(255), ActionMore nvarchar(max))
GO
