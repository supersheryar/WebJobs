SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbActions](
	[ActionId] [int] IDENTITY(1000,1) NOT NULL,
	[ActionName] [nvarchar](100) NOT NULL,
	[ActionType] [nvarchar](255) NOT NULL,
	[ActionMore] [nvarchar](max) NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_WJbActions] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UX_WJbActions_ActionName] UNIQUE NONCLUSTERED 
(
	[ActionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbHistory](
	[JobId] [int] NOT NULL,
	[JobPriority] [tinyint] NOT NULL,
	[Created] [datetime] NOT NULL,
	[RuleId] [int] NOT NULL,
	[Started] [datetime] NULL,
	[Finished] [datetime] NULL,
	[JobMore] [nvarchar](max) NULL,
 CONSTRAINT [PK_WJbHistory] PRIMARY KEY CLUSTERED 
(
	[JobId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbLogs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[LogLevel] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[LogMore] [nvarchar](max) NULL,
 CONSTRAINT [PK_WJbLogs] PRIMARY KEY CLUSTERED 
(
	[LogId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbQueue](
	[JobId] [int] IDENTITY(1,1) NOT NULL,
	[JobPriority] [tinyint] NOT NULL,
	[Created] [datetime] NOT NULL,
	[RuleId] [int] NOT NULL,
	[Started] [datetime] NULL,
	[Finished] [datetime] NULL,
	[JobMore] [nvarchar](max) NULL,
 CONSTRAINT [PK_WJbQueue] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbRules](
	[RuleId] [int] IDENTITY(1000,1) NOT NULL,
	[RuleName] [nvarchar](100) NOT NULL,
	[RulePriority] [tinyint] NOT NULL,
	[ActionId] [int] NOT NULL,
	[RuleMore] [nvarchar](max) NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_WJbRules] PRIMARY KEY CLUSTERED 
(
	[RuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[WJbSettings](
	[Name] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_WJbSettings] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_WJbHistory_CreatedDesc] ON [dbo].[WJbHistory]
(
	[Created] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WJbHistory_RuleId] ON [dbo].[WJbHistory]
(
	[RuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WJbLogs_LoggedDesc] ON [dbo].[WJbLogs]
(
	[Logged] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WJbLogs_LogLevel] ON [dbo].[WJbLogs]
(
	[LogLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_WJbLogs_Title] ON [dbo].[WJbLogs]
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WJbQueue_RuleId] ON [dbo].[WJbQueue]
(
	[RuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON

CREATE UNIQUE NONCLUSTERED INDEX [UX_WJbRules_RuleName] ON [dbo].[WJbRules]
(
	[RuleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
ALTER TABLE [dbo].[WJbActions] ADD  CONSTRAINT [DF_WJbActions_Disabled]  DEFAULT ((0)) FOR [Disabled]
ALTER TABLE [dbo].[WJbLogs] ADD  CONSTRAINT [DF_WJbLogs_Created]  DEFAULT (getdate()) FOR [Logged]
ALTER TABLE [dbo].[WJbQueue] ADD  CONSTRAINT [DF_WJbQueue_JobPriority]  DEFAULT ((2)) FOR [JobPriority]
ALTER TABLE [dbo].[WJbQueue] ADD  CONSTRAINT [DF_WJbQueue_Created]  DEFAULT (getdate()) FOR [Created]
ALTER TABLE [dbo].[WJbRules] ADD  CONSTRAINT [DF_WJbRules_Disabled]  DEFAULT ((0)) FOR [Disabled]
ALTER TABLE [dbo].[WJbRules] ADD  CONSTRAINT [DF_WJbRules_RulePriority]  DEFAULT ((2)) FOR [RulePriority]
ALTER TABLE [dbo].[WJbRules]  WITH CHECK ADD  CONSTRAINT [FK_WJbRules_WJbActions] FOREIGN KEY([ActionId])
REFERENCES [dbo].[WJbActions] ([ActionId])
ALTER TABLE [dbo].[WJbRules] CHECK CONSTRAINT [FK_WJbRules_WJbActions]