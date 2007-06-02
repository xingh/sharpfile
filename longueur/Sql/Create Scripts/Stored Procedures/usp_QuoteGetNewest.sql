USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_QuoteGetNewest]    Script Date: 06/02/2007 18:23:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_QuoteGetNewest] 
AS
	SET NOCOUNT ON
	
SELECT TOP 5 qt.QuoteText, q.QuoteID, a.ArtistName, s.SongName
FROM Quote q JOIN Artist a ON q.ArtistID = a.ArtistID JOIN Song s ON q.SongID = s.SongID JOIN QuoteText qt ON qt.QuoteID = q.QuoteID
WHERE (SELECT COUNT(qt2.QuoteTextID) FROM QuoteText qt2 WHERE qt2.QuoteID = q.QuoteID) = 1
ORDER BY qt.DateTimeStamp DESC;
	
	RETURN
