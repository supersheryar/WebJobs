BEGIN /***** Init Tables *****/

BEGIN /*** Init SshNet Actions ***/

SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 100))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (100, N'SshNet.GetFiles', N'SshNet.GetFilesAction, UkrGuru.WebJobs.Actions.SshNet', N'{
  "sshnet_options_name": "SshNetOptions",
  "remote_path": null,
  "local_path": null,
  "local_base_path": null
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 101))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (101, N'SshNet.PutFiles', N'SshNet.PutFilesAction, UkrGuru.WebJobs.Actions.SshNet', N'{
  "sshnet_options_name": "SshNetOptions",
  "remote_path": null,
  "local_path": null,
  "local_base_path": null,
  "local_move_path": "-delete"
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

END

BEGIN /*** Init SshNet Rules ***/

SET IDENTITY_INSERT [WJbRules] ON

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 100))
INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
VALUES (100, N'SshNet.GetFiles Base', 2, 100, N'{
  "remote_path": ".",
  "local_path": "test",
  "local_base_path": "C:\\"
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 101))
INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
VALUES (101, N'SshNet.PutFiles Base', 2, 101, N'{
  "remote_path": null,
  "local_path": "test",
  "local_base_path": "C:\\"
}', 0)

SET IDENTITY_INSERT [WJbRules] OFF

END

BEGIN /*** Init SshNet Settings ***/

IF NOT EXISTS (SELECT * FROM [dbo].[WJbSettings] WHERE [Name] = 'SshNetOptions')
INSERT [dbo].[WJbSettings] ([Name], [Value]) VALUES (N'SshNetOptions', N'{
	"Host": "domain.com",
	"Port": 22,
	"UserName": "test",
	"Password": "test"
}')

END

END