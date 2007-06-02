USE [Longueur]
GO
/****** Object:  Table [dbo].[Slog]    Script Date: 06/02/2007 18:18:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Slog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_Slog_DateTime]  DEFAULT (getdate()),
	[UserId] [int] NOT NULL,
	[Title] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF