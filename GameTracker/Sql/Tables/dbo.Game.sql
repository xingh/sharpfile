USE [GameTracker]
GO
/****** Object:  Table [dbo].[Game]    Script Date: 03/11/2007 23:34:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Game](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MatchId] [int] NULL,
 CONSTRAINT [PK_Game_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
