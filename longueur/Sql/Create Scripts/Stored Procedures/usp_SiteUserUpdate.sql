USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserUpdate]    Script Date: 06/02/2007 18:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SiteUserUpdate] 
	(
	@Name VARCHAR(255),
	@Email VARCHAR(255),
	@Id VARCHAR(255),
	@Password VARCHAR(255) = NULL,
	@Type INT = -1
	)
AS
	SET NOCOUNT ON
	
IF @Password IS NULL
BEGIN
	UPDATE SiteUser SET [Name] = @Name, Email = @Email WHERE Id = @Id;
END
ELSE
BEGIN
	UPDATE SiteUser SET [Name] = @Name, Email = @Email, Password = @Password WHERE Id = @Id;
END

IF @Type > -1
BEGIN
	UPDATE SiteUser SET Type = @Type WHERE Id = @Id;
END
ELSE
BEGIN
	UPDATE SiteUser SET Type = @Type WHERE Id = @Id;
END
	
	RETURN
