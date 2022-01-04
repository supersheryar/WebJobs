SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 5))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (5, N'RunApiProc', N'RunApiProcAction, UkrGuru.WebJobs.Actions', N'{	}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

SET IDENTITY_INSERT [WJbRules] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 5))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (5, N'RunApiProc Base', 2, 5, NULL, 0)

SET IDENTITY_INSERT [WJbRules] OFF
