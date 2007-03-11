USE [GameTracker]
GO
/****** Object:  Table [dbo].[Score]    Script Date: 03/11/2007 23:34:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Score](
	[GameId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Points] [int] NOT NULL,
 CONSTRAINT [PK_Score] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC,
	[PlayerId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
