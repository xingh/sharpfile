USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_CommentDelete]    Script Date: 06/02/2007 18:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CommentDelete] 
@CommentID int = NULL,
@UserID int = NULL

AS
	SET NOCOUNT ON
	
IF @CommentID IS NOT NULL AND @UserID IS NOT NULL
BEGIN
	IF (SELECT CommentID FROM Comment WHERE CommentID = @CommentID AND UserID = @UserID) IS NOT NULL
	BEGIN
		DELETE FROM Comment WHERE CommentID = @CommentID;
	END
	ELSE IF (SELECT UserRoleName FROM SiteUser s JOIN UserRole u ON s.UserRoleID = u.UserRoleID WHERE UserID = @UserID) = 'Admin'
	BEGIN
		IF (SELECT CommentID FROM Comment WHERE CommentID = @CommentID) IS NOT NULL
		BEGIN
			DELETE FROM Comment WHERE CommentID = @CommentID;
		END
	END
END	
	
	RETURN
