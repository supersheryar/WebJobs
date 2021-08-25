IF NOT EXISTS(SELECT * FROM [dbo].[WJbSettings] WHERE Name = N'StmpSettings')
    INSERT [dbo].[WJbSettings] ([Name], [Value]) VALUES (N'StmpSettings', N'{
    "from": "demo@demo.demo",
    "host": "demo.demo",
    "port": 25,
    "enableSsl": false,
    "userName": "demo",
    "password": "demo"
}')

SET IDENTITY_INSERT [dbo].[WJbActions] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[WJbActions] WHERE (ActionId = 3))
    INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) VALUES (3, N'FillTemplate', N'FillTemplateAction, UkrGuru.WebJobs.Actions', N'{
    "tname_pattern": "[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"
}', 0)
SET IDENTITY_INSERT [dbo].[WJbActions] OFF

SET IDENTITY_INSERT [dbo].[WJbRules] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[WJbRules] WHERE ([RuleId] = 6))
    INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) VALUES (6, N'Send Template Rule', 2, 3, N'{
  "template_subject": "Hello DEAR_NAME",
  "tvalue_DEAR_NAME": "John",
  "next": "5",
  "next_to": "test@test.test"
}', 0)
SET IDENTITY_INSERT [dbo].[WJbRules] OFF