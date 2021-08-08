SET IDENTITY_INSERT [dbo].[WJbActions] ON 
INSERT [dbo].[WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) VALUES (2, N'SendEmail', N'SendEmailAction, UkrGuru.WebJobs.Actions', N'{
  "smtp_settings_name": "StmpSettings",
  "from": "",
  "to": "",
  "cc": "",
  "bcc": "",
  "subject": "",
  "body": "",
  "attachment": "",
  "attachments": ""
}', 0)
SET IDENTITY_INSERT [dbo].[WJbActions] OFF

SET IDENTITY_INSERT [dbo].[WJbRules] ON 
INSERT [dbo].[WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) VALUES (5, N'Send Email Rule', 2, 2, N'{
  "to": "test@test.test",
  "subject": "test"
}
', 0)
SET IDENTITY_INSERT [dbo].[WJbRules] OFF

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[WJbFiles](
	[Id] [uniqueidentifier] NOT NULL,
	[Created] [datetime] NOT NULL,
	[FileName] [nvarchar](100) NULL,
	[FileContent] [varbinary](max) NULL,
 CONSTRAINT [PK_WJbFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_WJbFiles_CreatedDesc] ON [dbo].[WJbFiles]
(
	[Created] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Id]  DEFAULT (newid()) FOR [Id]

ALTER TABLE [dbo].[WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Created]  DEFAULT (getdate()) FOR [Created]

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbFiles_Ins]
    @Data nvarchar(max)
AS
DECLARE @Ids TABLE (Id uniqueidentifier);

INSERT INTO WJbFiles (FileName, FileContent)
OUTPUT INSERTED.Id
INTO @Ids (Id) 
SELECT * FROM OPENJSON(@Data) 
WITH (FileName nvarchar(100), FileContent varbinary(MAX))

DECLARE @Id uniqueidentifier = (SELECT TOP 1 Id FROM @Ids)

EXEC WJbFiles_Item @Id
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbFiles_Item]
	@Data uniqueidentifier
AS
SELECT Id, Created, FileName, FileContent
FROM WJbFiles
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Test]
	@Data varchar(10) 
AS
INSERT INTO WJbQueue (RuleId, Created, Started) 
VALUES (CAST(@Data AS int), GETDATE(), GETDATE())

SELECT SCOPE_IDENTITY()
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbJobs_Start]
    @Data varchar(10)
AS
DECLARE @JobId int = CAST(@Data AS int)

INSERT INTO WJbQueue (JobPriority, Created, RuleId, Started, Finished, JobMore, JobStatus)
SELECT JobPriority, GETDATE() Created, RuleId, Started, NULL Finished, JobMore, 0 JobStatus
FROM WJbHistory
WHERE (JobId = @JobId)

IF @@ROWCOUNT > 0 SET @JobId = SCOPE_IDENTITY()

UPDATE WJbQueue
SET Started = GETDATE(), JobStatus = 2 /* Running */
WHERE JobId = @JobId

EXEC WJbQueue_Item @JobId
';
