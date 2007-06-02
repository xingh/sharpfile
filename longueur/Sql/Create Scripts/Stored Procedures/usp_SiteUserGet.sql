USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SiteUserGet]    Script Date: 06/02/2007 18:23:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SiteUserGet]
AS
	SET NOCOUNT ON
	
SELECT	Id, [Name], Password, Email, s.Type, TypeName, [DateTime]
FROM	SiteUser s
	JOIN UserRole u ON s.Type = u.Type
	
	RETURN
