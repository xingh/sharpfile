USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserDelete]    Script Date: 06/02/2007 18:23:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SiteUserDelete] 
(
@AnonUserId int = NULL,
@Id int = NULL
)

AS
	SET NOCOUNT ON
	
IF @AnonUserId IS NOT NULL AND @Id IS NOT NULL
BEGIN
	UPDATE Quote SET UserId = @AnonUserId WHERE UserId = @Id;
	UPDATE Comment SET UserId = @AnonUserId WHERE UserId = @Id;
	UPDATE Rating SET UserId = @AnonUserId WHERE UserId = @Id;
	DELETE FROM SiteUser WHERE Id = @Id;
END
	 
	RETURN