USE [Longueur]
GO
/****** Object:  Table [dbo].[Rating]    Script Date: 06/02/2007 18:17:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rating](
	[QuoteID] [int] NOT NULL,
	[Rating] [smallint] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED 
(
	[QuoteID] ASC,
	[Rating] ASC,
	[UserID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
