USE [Longueur]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 06/02/2007 18:15:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Stacktrace] [varchar](1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IP] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Url] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateTime] [datetime] NULL CONSTRAINT [DF_ErrorLog_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF