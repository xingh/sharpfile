USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_UserRoleGet]    Script Date: 06/02/2007 18:26:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_UserRoleGet] 
AS
	SET NOCOUNT ON
	
SELECT	Type, TypeName
FROM	UserRole
ORDER BY Type ASC
	
	RETURN
