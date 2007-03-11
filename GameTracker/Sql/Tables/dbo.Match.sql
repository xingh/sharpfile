USE [GameTracker]
GO
/****** Object:  Table [dbo].[Match]    Script Date: 03/11/2007 23:34:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Match](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_Match_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_Match] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
