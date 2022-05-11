SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbActions]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbActions](
		[ActionId] [int] IDENTITY(1000,1) NOT NULL,
		[ActionName] [nvarchar](100) NOT NULL,
		[ActionType] [nvarchar](255) NOT NULL,
		[ActionMore] [nvarchar](max) NULL,
		[Disabled] [bit] NOT NULL,
	 CONSTRAINT [PK_WJbActions] PRIMARY KEY CLUSTERED 
	(
		[ActionId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	 CONSTRAINT [UX_WJbActions_ActionName] UNIQUE NONCLUSTERED 
	(
		[ActionName] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbFiles]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbFiles](
		[Id] [uniqueidentifier] NOT NULL,
		[Created] [datetime] NOT NULL,
		[FileName] [nvarchar](100) NULL,
		[FileContent] [varbinary](max) NULL,
	 CONSTRAINT [PK_WJbFiles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbHistory]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbHistory](
		[JobId] [int] NOT NULL,
		[JobPriority] [tinyint] NOT NULL,
		[Created] [datetime] NOT NULL,
		[RuleId] [int] NOT NULL,
		[Started] [datetime] NULL,
		[Finished] [datetime] NULL,
		[JobMore] [nvarchar](max) NULL,
		[JobStatus] [tinyint] NOT NULL,
	 CONSTRAINT [PK_WJbHistory] PRIMARY KEY CLUSTERED 
	(
		[JobId] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbLogs](
		[LogId] [int] IDENTITY(1,1) NOT NULL,
		[Logged] [datetime] NOT NULL,
		[LogLevel] [int] NOT NULL,
		[Title] [nvarchar](100) NOT NULL,
		[LogMore] [nvarchar](max) NULL,
	 CONSTRAINT [PK_WJbLogs] PRIMARY KEY CLUSTERED 
	(
		[LogId] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbQueue]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbQueue](
		[JobId] [int] IDENTITY(1000,1) NOT NULL,
		[JobPriority] [tinyint] NOT NULL,
		[Created] [datetime] NOT NULL,
		[RuleId] [int] NOT NULL,
		[Started] [datetime] NULL,
		[Finished] [datetime] NULL,
		[JobMore] [nvarchar](max) NULL,
		[JobStatus] [tinyint] NOT NULL,
	 CONSTRAINT [PK_WJbQueue] PRIMARY KEY CLUSTERED 
	(
		[JobId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbRules]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbRules](
		[RuleId] [int] IDENTITY(1000,1) NOT NULL,
		[RuleName] [nvarchar](100) NOT NULL,
		[RulePriority] [tinyint] NOT NULL,
		[ActionId] [int] NOT NULL,
		[RuleMore] [nvarchar](max) NULL,
		[Disabled] [bit] NOT NULL,
	 CONSTRAINT [PK_WJbRules] PRIMARY KEY CLUSTERED 
	(
		[RuleId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbSettings]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbSettings](
		[Name] [nvarchar](100) NOT NULL,
		[Value] [nvarchar](max) NULL,
	 CONSTRAINT [PK_WJbSettings] PRIMARY KEY CLUSTERED 
	(
		[Name] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbFiles]') AND name = N'IX_WJbFiles_CreatedDesc')
	CREATE NONCLUSTERED INDEX [IX_WJbFiles_CreatedDesc] ON [WJbFiles]
	(
		[Created] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbHistory]') AND name = N'IX_WJbHistory_CreatedDesc')
	CREATE NONCLUSTERED INDEX [IX_WJbHistory_CreatedDesc] ON [WJbHistory]
	(
		[Created] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbHistory]') AND name = N'IX_WJbHistory_RuleId')
	CREATE NONCLUSTERED INDEX [IX_WJbHistory_RuleId] ON [WJbHistory]
	(
		[RuleId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND name = N'IX_WJbLogs_LoggedDesc')
	CREATE NONCLUSTERED INDEX [IX_WJbLogs_LoggedDesc] ON [WJbLogs]
	(
		[Logged] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND name = N'IX_WJbLogs_LogLevel')
	CREATE NONCLUSTERED INDEX [IX_WJbLogs_LogLevel] ON [WJbLogs]
	(
		[LogLevel] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND name = N'IX_WJbLogs_Title')
	CREATE NONCLUSTERED INDEX [IX_WJbLogs_Title] ON [WJbLogs]
	(
		[Title] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbQueue]') AND name = N'IX_WJbQueue_RuleId')
	CREATE NONCLUSTERED INDEX [IX_WJbQueue_RuleId] ON [WJbQueue]
	(
		[RuleId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbRules]') AND name = N'UX_WJbRules_RuleName')
	CREATE UNIQUE NONCLUSTERED INDEX [UX_WJbRules_RuleName] ON [WJbRules]
	(
		[RuleName] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbActions_Disabled]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbActions] ADD  CONSTRAINT [DF_WJbActions_Disabled]  DEFAULT ((0)) FOR [Disabled]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbFiles_Id]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Id]  DEFAULT (newid()) FOR [Id]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbFiles_Created]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Created]  DEFAULT (getdate()) FOR [Created]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbHistory_JobStatus]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbHistory] ADD  CONSTRAINT [DF_WJbHistory_JobStatus]  DEFAULT ((0)) FOR [JobStatus]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbLogs_Created]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbLogs] ADD  CONSTRAINT [DF_WJbLogs_Created]  DEFAULT (getdate()) FOR [Logged]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbQueue_JobPriority]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbQueue] ADD  CONSTRAINT [DF_WJbQueue_JobPriority]  DEFAULT ((2)) FOR [JobPriority]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbQueue_Created]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbQueue] ADD  CONSTRAINT [DF_WJbQueue_Created]  DEFAULT (getdate()) FOR [Created]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbQueue_JobStatus]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbQueue] ADD  CONSTRAINT [DF_WJbQueue_JobStatus]  DEFAULT ((0)) FOR [JobStatus]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbRules_RulePriority]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbRules] ADD  CONSTRAINT [DF_WJbRules_RulePriority]  DEFAULT ((2)) FOR [RulePriority]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbRules_Disabled]') AND type = 'D')
BEGIN
	ALTER TABLE [WJbRules] ADD  CONSTRAINT [DF_WJbRules_Disabled]  DEFAULT ((0)) FOR [Disabled]
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_WJbRules_WJbActions]') AND parent_object_id = OBJECT_ID(N'[WJbRules]'))
	ALTER TABLE [WJbRules]  WITH CHECK ADD  CONSTRAINT [FK_WJbRules_WJbActions] FOREIGN KEY([ActionId]) REFERENCES [WJbActions] ([ActionId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_WJbRules_WJbActions]') AND parent_object_id = OBJECT_ID(N'[WJbRules]'))
	ALTER TABLE [WJbRules] CHECK CONSTRAINT [FK_WJbRules_WJbActions]
