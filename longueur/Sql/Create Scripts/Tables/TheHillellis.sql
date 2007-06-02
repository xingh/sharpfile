USE [Longueur]
GO
/****** Object:  Table [dbo].[TheHillellis]    Script Date: 06/02/2007 18:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TheHillellis](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_TheHillellies_DateTime]  DEFAULT (getdate()),
	[UserId] [int] NOT NULL,
	[Title] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TagId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF