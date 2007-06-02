USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchData]    Script Date: 06/02/2007 18:23:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SearchData] 
	@SearchString CHAR(1),
	@SearchType VARCHAR(20),
	@RowCount INT = 10,
	@Debug bit = 0 -- Will spit out the dynamic sql for troubleshooting purposes
AS

BEGIN
	SET NOCOUNT ON;

	/* Declaring some variables that will store the table and column info */
	DECLARE @ColumnName varchar(100);
	
	/* Used to check if the search type isn't screwy */
	DECLARE @ValidSearchType bit;
	SET @ValidSearchType = 0;
	
	/* Used to check if the query isn't screwy */
	DECLARE @ValidQuery bit;
	SET @ValidQuery = 0;

	/* Define our table and columns for the search we want to do */
	IF @SearchType = 'Song'
	BEGIN
		SET @ColumnName = 'SongName';
		SET @ValidSearchType = 1;
	END
	ELSE IF @SearchType = 'Artist'
	BEGIN
		SET @ColumnName = 'ArtistName';
		SET @ValidSearchType = 1;
	END
	ELSE IF @SearchType = 'Album'
	BEGIN
		SET @ColumnName = 'AlbumName';
		SET @ValidSearchType = 1;
	END

	/* As long as we have a valid search, start building our sql string */
	IF @ValidSearchType = 1
	BEGIN
		DECLARE @Sql nvarchar(4000);
		SET @Sql = N'
SELECT q.QuoteId, qt.QuoteText, a.ArtistName AS ArtistName, 
	CASE WHEN q.LyricistID IS NULL
	THEN a.ArtistName
	ELSE 
	(SELECT a.ArtistName FROM Artist a WHERE a.ArtistID = q.LyricistID)
	END AS LyricistName, 
	g.GenreName, s.SongName, al.AlbumName, ar.AvgRating, qt.DateTimeStamp
FROM Quote q
	JOIN QuoteText qt ON qt.QuoteID = q.QuoteID
	LEFT JOIN Artist a ON q.ArtistID = a.ArtistID
    LEFT JOIN Genre g ON q.GenreID = g.GenreID
    LEFT JOIN Album al ON q.AlbumID = al.AlbumID
    LEFT JOIN AvgRating ar ON q.QuoteID = ar.QuoteID
	LEFT JOIN Song s ON q.SongID = s.SongID';

		/* If we are looking for numerals append some good stuff to our sql string */
		IF @SearchString = '#'
		BEGIN
			SET @sql = @sql + N' 
WHERE 
	(' + @ColumnName + ' LIKE ''0%'' OR 
	' + @ColumnName + ' LIKE ''1%'' OR 
	' + @ColumnName + ' LIKE ''2%'' OR 
	' + @ColumnName + ' LIKE ''3%'' OR 
	' + @ColumnName + ' LIKE ''4%'' OR 
	' + @ColumnName + ' LIKE ''5%'' OR 
	' + @ColumnName + ' LIKE ''6%'' OR 
	' + @ColumnName + ' LIKE ''7%'' OR 
	' + @ColumnName + ' LIKE ''8%'' OR 
	' + @ColumnName + ' LIKE ''9%'')';

			SET @ValidQuery = 1;
		END
		/* Or if we are looking for alphas append some good stuff to our sql string */
		ELSE IF Quotes.dbo.udf_CheckCharIsLetter(@SearchString) = 1
		BEGIN
			SET @Sql = @Sql + N' 
WHERE ' + @ColumnName + ' LIKE ''' + @SearchString + '%'''

			SET @ValidQuery = 1;
		END

		/* If we are debugging print out our sql string */
		IF @Debug = 1 
		BEGIN
			PRINT @Sql
		END

		/* If everything looks fine and dandy, execute our string */
		IF @ValidQuery = 1
		BEGIN
			SET ROWCOUNT @RowCount;

			SET @Sql = @Sql + N'
AND qt.QuoteTextID =	(SELECT TOP 1 qt2.QuoteTextID 
						FROM QuoteText qt2
						WHERE qt2.QuoteID = q.QuoteID
						ORDER BY qt2.DateTimeStamp DESC)
ORDER BY a.ArtistName, al.AlbumName, s.SongName ASC;';

			EXEC sp_executesql @Sql, N'@ColumnName varchar(100), @SearchString varchar(100)', @ColumnName = @ColumnName, @SearchString = @SearchString

			SET ROWCOUNT 0;
		END
	END
END


