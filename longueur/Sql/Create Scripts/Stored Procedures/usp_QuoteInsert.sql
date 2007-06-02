USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_QuoteInsert]    Script Date: 06/02/2007 18:23:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_QuoteInsert]
	(
	@QuoteText varchar(255),
	@ArtistName varchar(255) = NULL,
	@LyricistName varchar(255) = NULL,
	@SongName varchar(255) = NULL,
	@AlbumName varchar(255) = NULL,
	@GenreName varchar(255) = NULL,
	@UserID int
	)
AS
	SET NOCOUNT ON

DECLARE @ArtistID int, @LyricistID int, @SongID int, @AlbumID int, @GenreID int, @QuoteID int;

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

/* get lyricist id */
IF @LyricistName IS NOT NULL
BEGIN
	IF (SELECT COUNT(ArtistName) FROM Artist WHERE ArtistName = @LyricistName) > 0
	BEGIN
		SET @LyricistID = (SELECT ArtistID FROM Artist WHERE ArtistName = @LyricistName);
	END
	ELSE
	BEGIN
		INSERT INTO Artist (ArtistName) VALUES (@LyricistName);
		SET @LyricistID = (SELECT SCOPE_IDENTITY());
	END
END

/* get song id */
IF @SongName IS NOT NULL
BEGIN
	IF (SELECT COUNT(SongName) FROM Song WHERE SongName = @SongName) > 0
	BEGIN
		SET @SongID = (SELECT SongID FROM Song WHERE SongName = @SongName);
	END
	ELSE
	BEGIN
		INSERT INTO Song (SongName) VALUES (@SongName);
		SET @SongID = (SELECT SCOPE_IDENTITY());
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

/* get genre id */
IF @GenreName IS NOT NULL
BEGIN
	IF (SELECT COUNT(GenreName) FROM Genre WHERE GenreName = @GenreName) > 0
	BEGIN
		SET @AlbumID = (SELECT GenreID FROM Genre WHERE GenreName = @GenreName);
	END
	ELSE
	BEGIN
		INSERT INTO Genre (GenreName) VALUES (@GenreName);
		SET @GenreID = (SELECT SCOPE_IDENTITY());
	END
END
	
BEGIN TRANSACTION
	INSERT INTO Quote (ArtistID, LyricistID, SongID, AlbumID, GenreID) VALUES (@ArtistID, @LyricistID, @SongID, @AlbumID, @GenreID);
	SET @QuoteID = (SELECT SCOPE_IDENTITY());
	
	INSERT INTO QuoteText(QuoteText, QuoteID, UserID, DateTimeStamp) VALUES (@QuoteText, @QuoteID, @UserID, getdate());
	SELECT @QuoteID;
COMMIT TRANSACTION
	
	RETURN
