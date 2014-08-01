/****** Object:  Table [dbo].[DiscusCategories]    Script Date: 08/22/2012 11:36:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscusCategories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[Name] [nvarchar](64) NULL,
 CONSTRAINT [PK_DiscusCategories] PRIMARY KEY CLUSTERED
(
	[CategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ActionLog]    Script Date: 08/22/2012 11:36:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionLog](
	[ActionLogId] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Significance] [tinyint] NOT NULL,
	[UserId] [int] NULL,
	[Action] [nvarchar](32) NOT NULL,
	[AdditionalInfo] [nvarchar](max) NULL,
 CONSTRAINT [PK_ActionLog] PRIMARY KEY CLUSTERED
(
	[ActionLogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[DiscusThreads]    Script Date: 08/22/2012 11:36:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscusThreads](
	[ThreadId] [int] IDENTITY(1,1) NOT NULL,
	[ThreadName] [nvarchar](32) NULL,
	[CategoryId] [int] NULL,
	[IsLocked] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[AuthorUserId] [int] NULL,
	[AuthorNameNonRegistered] [nvarchar](32) NULL,
	[Title] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_DiscusThreads] PRIMARY KEY CLUSTERED
(
	[ThreadId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



/****** Object:  Table [dbo].[DiscusMessages]    Script Date: 08/22/2012 11:36:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscusMessages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[ThreadId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[AuthorUserId] [int] NULL,
	[AuthorNameNonRegistered] [nvarchar](32) NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_DiscusMessages] PRIMARY KEY CLUSTERED
(
	[MessageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  ForeignKey [FK_ActionLog_Users]    Script Date: 08/22/2012 11:36:05 ******/
ALTER TABLE [dbo].[ActionLog]  WITH CHECK ADD  CONSTRAINT [FK_ActionLog_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ActionLog] CHECK CONSTRAINT [FK_ActionLog_Users]
GO
/****** Object:  ForeignKey [FK_DiscusMessages_DiscusThreads]    Script Date: 08/22/2012 11:36:05 ******/
ALTER TABLE [dbo].[DiscusMessages]  WITH CHECK ADD  CONSTRAINT [FK_DiscusMessages_DiscusThreads] FOREIGN KEY([ThreadId])
REFERENCES [dbo].[DiscusThreads] ([ThreadId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DiscusMessages] CHECK CONSTRAINT [FK_DiscusMessages_DiscusThreads]
GO
/****** Object:  ForeignKey [FK_DiscusMessages_Users]    Script Date: 08/22/2012 11:36:05 ******/
ALTER TABLE [dbo].[DiscusMessages]  WITH CHECK ADD  CONSTRAINT [FK_DiscusMessages_Users] FOREIGN KEY([AuthorUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DiscusMessages] CHECK CONSTRAINT [FK_DiscusMessages_Users]
GO
/****** Object:  ForeignKey [FK_DiscusThreads_DiscusCategories]    Script Date: 08/22/2012 11:36:05 ******/
ALTER TABLE [dbo].[DiscusThreads]  WITH CHECK ADD  CONSTRAINT [FK_DiscusThreads_DiscusCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[DiscusCategories] ([CategoryId])
ON UPDATE SET NULL
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[DiscusThreads] CHECK CONSTRAINT [FK_DiscusThreads_DiscusCategories]
GO
/****** Object:  ForeignKey [FK_DiscusThreads_Users]    Script Date: 08/22/2012 11:36:05 ******/
ALTER TABLE [dbo].[DiscusThreads]  WITH CHECK ADD  CONSTRAINT [FK_DiscusThreads_Users] FOREIGN KEY([AuthorUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DiscusThreads] CHECK CONSTRAINT [FK_DiscusThreads_Users]
GO
