USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_CommentInsert]    Script Date: 06/02/2007 18:21:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CommentInsert] 
@QuoteID int = NULL,
@UserID int = NULL,
@CommentText varchar(8000)

AS
	SET NOCOUNT ON
	
IF @QuoteID IS NOT NULL AND @UserID IS NOT NULL AND @CommentText IS NOT NULL
BEGIN
	INSERT INTO Comment (QuoteID, UserID, CommentText, DateTimeStamp) VALUES (@QuoteID, @UserID, @CommentText, getdate());
END
	
	RETURN
