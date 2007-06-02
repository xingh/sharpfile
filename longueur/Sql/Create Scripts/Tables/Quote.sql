USE [Longueur]
GO
/****** Object:  Table [dbo].[Quote]    Script Date: 06/02/2007 18:17:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quote](
	[QuoteID] [int] IDENTITY(1,1) NOT NULL,
	[GenreID] [int] NULL,
	[ArtistID] [int] NULL,
	[LyricistID] [int] NULL,
	[AlbumID] [int] NULL,
	[SongID] [int] NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_Quote] PRIMARY KEY CLUSTERED 
(
	[QuoteID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
