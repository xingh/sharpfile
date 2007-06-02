USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_RatingDelete]    Script Date: 06/02/2007 18:23:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_RatingDelete]
@QuoteID int = NULL,
@UserID int = NULL

AS
	SET NOCOUNT ON
	
IF @QuoteID IS NOT NULL AND @UserID IS NOT NULL
BEGIN
	IF (SELECT Rating FROM Rating WHERE QuoteID = @QuoteID AND UserID = @UserID) IS NOT NULL
	BEGIN
		DELETE FROM Rating WHERE QuoteID = @QuoteID AND UserID = @UserID;
	END
END
	
	RETURN
