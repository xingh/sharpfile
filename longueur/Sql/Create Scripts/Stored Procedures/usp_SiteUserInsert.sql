USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserInsert]    Script Date: 06/02/2007 18:24:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SiteUserInsert] 
	(
	@Name VARCHAR(255),
	@Email VARCHAR(255),
	@Password VARCHAR(255),
	@Type INT = 2
	)
AS
	SET NOCOUNT ON
	
INSERT INTO SiteUser ([Name], Email, Password, Type) 
VALUES (@Name, @Email, @Password, @Type);

SELECT SCOPE_IDENTITY();
	
	RETURN
