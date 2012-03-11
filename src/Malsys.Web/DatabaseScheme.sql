
/****** Object:  Table [dbo].[Users]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [INT] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](32) NOT NULL,
	[NameLowercase] [NVARCHAR](32) NOT NULL,
	[PasswordHash] [BINARY](64) NOT NULL,
	[PasswordSalt] [BINARY](64) NOT NULL,
	[Email] [NVARCHAR](MAX) NOT NULL,
	[RegistrationDate] [DATETIME] NOT NULL,
	[LastLoginDate] [DATETIME] NOT NULL,
	[LastActivityDate] [DATETIME] NOT NULL,
	[LastPwdChangeDate] [DATETIME] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Name] ON [dbo].[Users] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_NameLowercase] ON [dbo].[Users] 
(
	[NameLowercase] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[CanonicInputs]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CanonicInputs](
	[CanonicInputId] [INT] IDENTITY(1,1) NOT NULL,
	[HASH] [BINARY](16) NOT NULL,
	[Source] [NVARCHAR](MAX) NOT NULL,
	[SourceSize] [INT] NOT NULL,
	[OutputSize] [BIGINT] NOT NULL,
 CONSTRAINT [PK_CanonicLsystems] PRIMARY KEY CLUSTERED 
(
	[CanonicInputId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CanonicLsystems_Hash] ON [dbo].[CanonicInputs] 
(
	[HASH] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Roles]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [INT] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](32) NOT NULL,
	[NameLowercase] [NVARCHAR](32) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name] ON [dbo].[Roles] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_NameLowercase] ON [dbo].[Roles] 
(
	[NameLowercase] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[InputProcesses]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InputProcesses](
	[InputProcessId] [INT] IDENTITY(1,1) NOT NULL,
	[ParentInputProcessId] [INT] NULL,
	[CanonicInputId] [INT] NOT NULL,
	[UserId] [INT] NULL,
	[ProcessDate] [DATETIME] NOT NULL,
	[Duration] [BIGINT] NOT NULL,
 CONSTRAINT [PK_LsystemProcesses] PRIMARY KEY CLUSTERED 
(
	[InputProcessId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InputProcesses_ProcessDate_Asc] ON [dbo].[InputProcesses] 
(
	[ProcessDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Duration of compiling and processing in ticks.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'InputProcesses', @level2type=N'COLUMN',@level2name=N'Duration'
GO

/****** Object:  Table [dbo].[Feedbacks]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedbacks](
	[FeedbackId] [INT] IDENTITY(1,1) NOT NULL,
	[UserId] [INT] NULL,
	[Subject] [NVARCHAR](MAX) NOT NULL,
	[SubmitDate] [DATETIME] NOT NULL,
	[Email] [NVARCHAR](MAX) NULL,
	[MESSAGE] [NVARCHAR](MAX) NOT NULL,
	[IsNew] [BIT] NOT NULL,
 CONSTRAINT [PK_Feedbacks] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[UsersInRoles]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersInRoles](
	[UserId] [INT] NOT NULL,
	[RoleId] [INT] NOT NULL,
 CONSTRAINT [PK_UsersInRoles] PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[SavedInputs]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedInputs](
	[SavedInputId] [INT] IDENTITY(1,1) NOT NULL,
	[RandomId] [NVARCHAR](8) NOT NULL,
	[UserId] [INT] NOT NULL,
	[ParentInputProcessId] [INT] NULL,
	[DATE] [DATETIME] NOT NULL,
	[Source] [NVARCHAR](MAX) NOT NULL,
	[SourceSize] [INT] NOT NULL,
	[OutputSize] [BIGINT] NOT NULL,
	[Duration] [BIGINT] NOT NULL,
	[VIEWS] [INT] NOT NULL,
 CONSTRAINT [PK_SavedInputs] PRIMARY KEY CLUSTERED 
(
	[SavedInputId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RandomId] ON [dbo].[SavedInputs] 
(
	[RandomId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[SavedInputs] 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ProcessOutputs]    Script Date: 03/11/2012 17:24:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOutputs](
	[ProcessOutputId] [INT] IDENTITY(1,1) NOT NULL,
	[InputProcessId] [INT] NULL,
	[FileName] [NVARCHAR](64) NOT NULL,
	[CreationDate] [DATETIME] NOT NULL,
	[LastOpenDate] [DATETIME] NOT NULL,
 CONSTRAINT [PK_ProcessOutputs] PRIMARY KEY CLUSTERED 
(
	[ProcessOutputId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProcessOutputs_FileName] ON [dbo].[ProcessOutputs] 
(
	[FileName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProcessOutputs_LastOpenDate_Asc] ON [dbo].[ProcessOutputs] 
(
	[LastOpenDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  ForeignKey [FK_InputProcesses_CanonicInputs]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[InputProcesses]  WITH CHECK ADD  CONSTRAINT [FK_InputProcesses_CanonicInputs] FOREIGN KEY([CanonicInputId])
REFERENCES [dbo].[CanonicInputs] ([CanonicInputId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InputProcesses] CHECK CONSTRAINT [FK_InputProcesses_CanonicInputs]
GO

/****** Object:  ForeignKey [FK_InputProcesses_InputProcesses]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[InputProcesses]  WITH CHECK ADD  CONSTRAINT [FK_InputProcesses_InputProcesses] FOREIGN KEY([ParentInputProcessId])
REFERENCES [dbo].[InputProcesses] ([InputProcessId])
GO
ALTER TABLE [dbo].[InputProcesses] CHECK CONSTRAINT [FK_InputProcesses_InputProcesses]
GO

/****** Object:  ForeignKey [FK_InputProcesses_Users]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[InputProcesses]  WITH CHECK ADD  CONSTRAINT [FK_InputProcesses_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InputProcesses] CHECK CONSTRAINT [FK_InputProcesses_Users]
GO

/****** Object:  ForeignKey [FK_Feedbacks_Users]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD  CONSTRAINT [FK_Feedbacks_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feedbacks] CHECK CONSTRAINT [FK_Feedbacks_Users]
GO

/****** Object:  ForeignKey [FK_UsersInRoles_Roles]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Roles]
GO

/****** Object:  ForeignKey [FK_UsersInRoles_Users]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Users]
GO

/****** Object:  ForeignKey [FK_SavedInputs_InputProcesses]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[SavedInputs]  WITH CHECK ADD  CONSTRAINT [FK_SavedInputs_InputProcesses] FOREIGN KEY([ParentInputProcessId])
REFERENCES [dbo].[InputProcesses] ([InputProcessId])
ON UPDATE SET NULL
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[SavedInputs] CHECK CONSTRAINT [FK_SavedInputs_InputProcesses]
GO

/****** Object:  ForeignKey [FK_SavedInputs_Users]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[SavedInputs]  WITH CHECK ADD  CONSTRAINT [FK_SavedInputs_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[SavedInputs] CHECK CONSTRAINT [FK_SavedInputs_Users]
GO

/****** Object:  ForeignKey [FK_ProcessOutputs_InputProcesses]    Script Date: 03/11/2012 17:24:40 ******/
ALTER TABLE [dbo].[ProcessOutputs]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOutputs_InputProcesses] FOREIGN KEY([InputProcessId])
REFERENCES [dbo].[InputProcesses] ([InputProcessId])
ON UPDATE SET NULL
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ProcessOutputs] CHECK CONSTRAINT [FK_ProcessOutputs_InputProcesses]
GO
