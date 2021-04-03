SET IDENTITY_INSERT [dbo].[WJbActions] ON 

IF NOT EXISTS (SELECT * FROM [dbo].[WJbActions] WHERE Id = 1)
	INSERT [dbo].[WJbActions] ([Id], [Name], [Disabled], [Type], [MoreJson]) VALUES (1, N'RunSqlProc', 0, N'SqlProcAction, UkrGuru.WebJobs', N'{
	  "proc": null,
	  "data": null
	}')
SET IDENTITY_INSERT [dbo].[WJbActions] OFF
SET IDENTITY_INSERT [dbo].[WJbRules] ON 

IF NOT EXISTS (SELECT * FROM [dbo].[WJbRules] WHERE Id = 1)
	INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (1, N'Cron Rule', 0, 2, 1, N'{
	  "cron": "* * * * *",
	  "proc": "Create_TestJobs"
	}')
IF NOT EXISTS (SELECT * FROM [dbo].[WJbRules] WHERE Id = 2)
	INSERT [dbo].[WJbRules] ([Id], [Name], [Disabled], [Priority], [ActionId], [MoreJson]) VALUES (2, N'Trigger Rule', 0, 2, 1, N'{
	  "proc": "Delay"
	}')
SET IDENTITY_INSERT [dbo].[WJbRules] OFF
