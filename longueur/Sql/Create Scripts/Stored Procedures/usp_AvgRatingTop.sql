USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_AvgRatingTop]    Script Date: 06/02/2007 18:21:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_AvgRatingTop]
AS
	SET NOCOUNT ON
	
SELECT TOP 1 q.QuoteID, qt.QuoteText, a.ArtistName, ar.AvgRating, s.SongName, al.AlbumName
FROM AvgRating ar JOIN Quote q ON ar.QuoteID = q.QuoteID JOIN Song s ON q.SongID = s.SongID JOIN QuoteText qt ON qt.QuoteID = q.QuoteID
LEFT JOIN Artist a ON q.ArtistID = a.ArtistID LEFT JOIN Album al ON al.AlbumID = q.AlbumID
ORDER BY AvgRating DESC;
	
	RETURN
