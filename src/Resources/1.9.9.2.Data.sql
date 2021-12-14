SET IDENTITY_INSERT [WJbActions] ON 
IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 1))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (1, N'RunSqlProc', N'RunSqlProcAction, UkrGuru.WebJobs', N'{
	"proc": "",
	"data": null,
	"timeout": null,
	"result_name": null
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 2))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (2, N'SendEmail', N'SendEmailAction, UkrGuru.WebJobs', N'{
	"smtp_settings_name": "StmpSettings",
	"from": "",
	"to": "",
	"cc": null,
	"bcc": null,
	"subject": "",
	"body": "",
	"attachment": null,
	"attachments": null
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 3))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (3, N'FillTemplate', N'FillTemplateAction, UkrGuru.WebJobs', N'{
	"tname_pattern": "[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

SET IDENTITY_INSERT [WJbRules] ON 
IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 1))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (1, N'RunSqlProc Base', 2, 1, NULL, 0)

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 2))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (2, N'SendEmail Base', 2, 2, NULL, 0)

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 3))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (3, N'FillTemplate Base', 2, 3, NULL, 0)

SET IDENTITY_INSERT [WJbRules] OFF
