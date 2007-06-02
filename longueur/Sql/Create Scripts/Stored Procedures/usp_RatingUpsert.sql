USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_RatingUpsert]    Script Date: 06/02/2007 18:23:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_RatingUpsert]
@QuoteID int = NULL,
@Rating int = NULL, 
@UserID int = NULL

AS
	SET NOCOUNT ON
	
IF @QuoteID IS NOT NULL AND @Rating IS NOT NULL AND @UserID IS NOT NULL
BEGIN
	IF (SELECT Rating FROM Rating WHERE QuoteID = @QuoteID AND UserID = @UserID) IS NULL
	BEGIN
		INSERT INTO Rating (QuoteID, Rating, UserID) VALUES (@QuoteID, @Rating, @UserID);
	END
	ELSE
	BEGIN
		UPDATE Rating SET Rating = @Rating WHERE QuoteID = @QuoteID AND UserID = @UserID;
	END
END
	
	RETURN
