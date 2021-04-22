SET IDENTITY_INSERT [dbo].[WJbActions] ON 

INSERT [dbo].[WJbActions] ([Id], [Name], [Disabled], [Type], [MoreJson]) VALUES (1, N'RunSqlProc', 0, N'SqlProcAction, UkrGuru.WebJobs', N'{
  "proc": "",
  "data": null,
  "timeout": null,
  "result_name": null
}')
INSERT [dbo].[WJbActions] ([Id], [Name], [Disabled], [Type], [MoreJson]) VALUES (1000, N'RunYourSqlProc', 0, N'WebJobsDemo.Actions.YourSqlProcAction, WebJobsDemo', N'{
  "proc": "",
  "data": null,
  "timeout": null,
  "result_name": null
}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF
SET IDENTITY_INSERT [dbo].[WJbRules] ON 

INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (1, N'Cron Rule', 0, 2, 1, N'{
  "cron": "* * * * *",
  "proc": "Jobs_Ins_Demo"
}')
INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (2, N'Trigger Rule', 0, 2, 1, N'{
  "proc": "Delay_Demo"
}')
INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (3, N'Complex Rule', 0, 2, 1, N'{
  "cron": "* * * * *",
  "proc": "Proc1_Demo",
  "result_name": "next_data",
  "next_rule": "4"
}')
INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (4, N'Next Rule', 0, 2, 1, N'{
  "proc": "Proc2_Demo",
  "result_name": "result"
}')
INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (1000, N'Your Rule', 0, 2, 1000, N'{
  "proc": "Delay_Demo",
  "data": "7"
}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
