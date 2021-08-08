SET IDENTITY_INSERT [dbo].[WJbActions] ON 
INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [Disabled], [ActionType], [ActionMore]) VALUES (1, N'RunSqlProc', 0, N'SqlProcAction, UkrGuru.WebJobs', N'{
  "proc": "",
  "data": null,
  "timeout": null,
  "result_name": null
}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF

SET IDENTITY_INSERT [dbo].[WJbRules] ON 
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (1, N'Cron Rule', 0, 2, 1, N'{
  "cron": "* * * * *",
  "proc": "Jobs_Ins_Demo"
}')
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (2, N'Trigger Rule', 0, 2, 1, N'{
  "proc": "Delay_Demo"
}')
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (3, N'Complex Rule', 0, 2, 1, N'{
  "cron": "* * * * *",
  "proc": "Proc1_Demo",
  "result_name": "next_data",
  "next": "4"
}')
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [Disabled], [RulePriority], [ActionId], [RuleMore]) VALUES (4, N'Next Rule', 0, 2, 1, N'{
  "proc": "Proc2_Demo",
  "result_name": "result"
}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
