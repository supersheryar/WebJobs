SET IDENTITY_INSERT [dbo].[WJbActions] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[WJbActions] WHERE (ActionId = 4))
    INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) VALUES (4, N'Url2Html', N'Url2HtmlAction, UkrGuru.WebJobs.Actions', N'{
  "url": "",
  "result_name": "next_body"
}', 0)
SET IDENTITY_INSERT [dbo].[WJbActions] OFF

SET IDENTITY_INSERT [dbo].[WJbRules] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[WJbRules] WHERE ([RuleId] = 7))
    INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) VALUES (7, N'Url2Html Rule', 2, 4, N'{
  "url": "https://ukrguru.com/",
  "next": "5",
  "next_subject": "ukrguru.com snapshot"
}', 0)
SET IDENTITY_INSERT [dbo].[WJbRules] OFF

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbFiles_Ins]
    @Data nvarchar(max)
AS
DECLARE @Ids TABLE (Id uniqueidentifier);

INSERT INTO WJbFiles (FileName, FileContent)
OUTPUT INSERTED.Id
INTO @Ids (Id) 
SELECT * FROM OPENJSON(@Data) 
WITH (FileName nvarchar(100), FileContent varbinary(MAX))

SELECT TOP 1 Id FROM @Ids
';