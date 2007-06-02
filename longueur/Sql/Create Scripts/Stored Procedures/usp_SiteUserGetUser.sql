USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserGetUser]    Script Date: 06/02/2007 18:24:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SiteUserGetUser] 
	(
	@Id int = NULL,
	@Name varchar(255) = NULL
	)
AS
	SET NOCOUNT ON
	
IF @Id IS NOT NULL
BEGIN
	SELECT u.*, r.*
	FROM SiteUser u JOIN UserRole r ON u.Type = r.Type
	WHERE Id = @Id;
END
ELSE IF @Name IS NOT NULL
BEGIN
	SELECT u.*, r.*
	FROM SiteUser u JOIN UserRole r ON u.Type = r.Type
	WHERE [Name] = @Name;
END
	
	RETURN

