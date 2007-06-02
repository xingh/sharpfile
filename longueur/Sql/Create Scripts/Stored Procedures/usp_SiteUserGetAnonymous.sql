USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserGetAnonymous]    Script Date: 06/02/2007 18:24:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SiteUserGetAnonymous] 
AS
	SET NOCOUNT ON
	
SELECT u.*, r.*
FROM SiteUser u JOIN UserRole r ON u.Type = r.Type
WHERE [Name] = 'Anonymous';
	
	RETURN