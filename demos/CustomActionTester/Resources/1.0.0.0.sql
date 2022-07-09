SET IDENTITY_INSERT [dbo].[WJbActions] ON 
IF NOT EXISTS (SELECT * FROM [dbo].[WJbActions] WHERE [ActionId] = 1000)
INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) VALUES (1000, N'SendMyEmail', N'SendMyEmailAction, CustomActionTester', N'{
	"smtp_settings_name": "MyStmpSettings",
	"from": null,
	"to": "",
	"cc": null,
	"bcc": null,
	"subject": null,
	"body": null,
	"attachment": null,
	"attachments": null
}', 0)
SET IDENTITY_INSERT [dbo].[WJbActions] OFF

SET IDENTITY_INSERT [dbo].[WJbRules] ON 
IF NOT EXISTS (SELECT * FROM [dbo].[WJbRules] WHERE [RuleId] = 1000)
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) VALUES (1000, N'SendMyEmail Test', 2, 1000, N'{
	"to": "test@test.com",
	"subject": "WebJobs Test #1"
}', 0)
SET IDENTITY_INSERT [dbo].[WJbRules] OFF

IF NOT EXISTS (SELECT * FROM [dbo].[WJbSettings] WHERE [Name] = 'MyStmpSettings')
INSERT [dbo].[WJbSettings] ([Name], [Value]) VALUES (N'MyStmpSettings', N'{
  "from": "test@test.com",
  "host": "smtp.test.com",
  "port": 587,
  "enableSsl": true,
  "userName": "test@test.com",
  "password": "12345"
}')

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Test]
	@Data int 
AS
INSERT INTO WJbQueue (RuleId) VALUES (@Data)

SELECT CAST(SCOPE_IDENTITY() as varchar)
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start]
    @Data int
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE JobId = @Data

EXEC WJbQueue_Get @Data
';
