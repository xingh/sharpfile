USE [Longueur]
GO
/****** Object:  Table [dbo].[Quote]    Script Date: 06/02/2007 18:17:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Href] [varchar](255) NOT NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [PK_Link] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]