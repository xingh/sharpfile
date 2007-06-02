USE [Longueur]
GO
/****** Object:  Table [dbo].[SiteUser]    Script Date: 06/02/2007 18:17:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SiteUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Password] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Email] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_User_UserRoleID]  DEFAULT ((2)),
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_SiteUser_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_SiteUser] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF