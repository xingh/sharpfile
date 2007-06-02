USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_AlbumGetDetails]    Script Date: 06/02/2007 18:20:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_AlbumGetDetails]
	(
	@ArtistName varchar(255) = NULL,
	@AlbumName varchar(255) = NULL
	)
AS
	SET NOCOUNT ON

DECLARE @ArtistID int, @AlbumID int;

/* get artist id */
IF @ArtistName IS NOT NULL
BEGIN
	IF (SELECT COUNT(ArtistName) FROM Artist WHERE ArtistName = @ArtistName) > 0
	BEGIN
		SET @ArtistID = (SELECT ArtistID FROM Artist WHERE ArtistName = @ArtistName);
	END
	ELSE
	BEGIN
		INSERT INTO Artist (ArtistName) VALUES (@ArtistName);
		SET @ArtistID = (SELECT SCOPE_IDENTITY());
	END
END

/* get album id */
IF @AlbumName IS NOT NULL
BEGIN
	IF (SELECT COUNT(AlbumName) FROM Album WHERE AlbumName = @AlbumName) > 0
	BEGIN
		SET @AlbumID = (SELECT AlbumID FROM Album WHERE AlbumName = @AlbumName);
	END
	ELSE
	BEGIN
		INSERT INTO Album (AlbumName) VALUES (@AlbumName);
		SET @AlbumID = (SELECT SCOPE_IDENTITY());
	END
END

/* get albums */
SELECT ar.ArtistName, al.AlbumName, al.AlbumImg
FROM Album al JOIN Artist ar ON al.ArtistID = ar.ArtistID
WHERE ar.ArtistID = @ArtistID;

/* get tracks */
SELECT ar.ArtistName, al.AlbumName, s.SongName 
FROM Song s JOIN Artist ar ON s.ArtistID = ar.ArtistID JOIN Album al ON s.AlbumID = al.AlbumID
WHERE s.ArtistID = @ArtistID AND s.AlbumID = @AlbumID;
	
	RETURN
