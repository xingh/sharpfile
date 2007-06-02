USE [Longueur]
GO
/****** Object:  Table [dbo].[TheHillellis_Archive]    Script Date: 06/02/2007 18:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TheHillellis_Archive](
	[ArchiveId] [int] NOT NULL,
	[EntryId] [int] NOT NULL,
 CONSTRAINT [PK_TheHillellis_Archive] PRIMARY KEY CLUSTERED 
(
	[ArchiveId] ASC,
	[EntryId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
