USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_QuoteGetComments]    Script Date: 06/02/2007 18:22:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_QuoteGetComments] 
	(
	@QuoteID int
	)
AS
	SET NOCOUNT ON
	
SELECT c.QuoteID, CommentID, UserName, u.UserID, CommentText, DateTimeStamp, LastEditedDateTimeStamp
FROM Comment c JOIN SiteUser u ON c.UserID = u.UserID
WHERE c.QuoteID = @QuoteID
ORDER BY DateTimeStamp DESC;
	
	RETURN
