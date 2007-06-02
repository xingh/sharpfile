USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_CommentUpdate]    Script Date: 06/02/2007 18:21:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CommentUpdate] 
@CommentID int = NULL,
@UserID int = NULL,
@CommentText varchar(8000)

AS
	SET NOCOUNT ON
	
IF @CommentID IS NOT NULL AND @CommentText IS NOT NULL AND @UserID IS NOT NULL
BEGIN
	IF (SELECT UserID FROM Comment WHERE CommentID = @CommentID) = @UserID OR (SELECT UserRoleName FROM SiteUser s JOIN UserRole u ON s.UserRoleID = u.UserRoleID WHERE UserID = @UserID) = 'Admin'
	BEGIN
		UPDATE Comment SET CommentText = @CommentText, LastEditedDateTimeStamp = getdate() WHERE CommentID = @CommentID;
	END
END
	
	RETURN