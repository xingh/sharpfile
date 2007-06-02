USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_QuoteGetDetails]    Script Date: 06/02/2007 18:22:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_QuoteGetDetails]
	(
	@UserID int,
	@QuoteID int
	)
AS
	SET NOCOUNT ON
	
SELECT TOP 1 qt.UserID, u.UserName, qt.QuoteText, a.ArtistName AS ArtistName, 
	CASE WHEN q.LyricistID IS NULL
	THEN a.ArtistName
	ELSE 
	(SELECT a.ArtistName FROM Artist a WHERE a.ArtistID = q.LyricistID)
	END AS LyricistName, 
	g.GenreName, s.SongName, al.AlbumName, r.Rating, ar.AvgRating, qt.DateTimeStamp
FROM Quote q
	JOIN QuoteText qt ON qt.QuoteID = q.QuoteID
	JOIN SiteUser u ON u.UserID = qt.UserID 
	LEFT JOIN Artist a ON q.ArtistID = a.ArtistID
    LEFT JOIN Genre g ON q.GenreID = g.GenreID
    LEFT JOIN Album al ON q.AlbumID = al.AlbumID
    LEFT JOIN AvgRating ar ON q.QuoteID = ar.QuoteID
	LEFT JOIN Song s ON q.SongID = s.SongID
	LEFT JOIN Rating r ON q.QuoteID = r.QuoteID AND r.UserID = @UserID
WHERE q.QuoteID = @QuoteID
ORDER BY qt.DateTimeStamp DESC;
	
	RETURN
