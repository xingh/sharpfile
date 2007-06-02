USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SlogGet]    Script Date: 06/02/2007 18:24:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SlogGet] 
AS
	SET NOCOUNT ON
	
SELECT	s.Id, Content, s.[DateTime], u.Id AS 'UserId', u.[Name], s.Title
FROM	Slog s
	JOIN SiteUser u ON s.UserId = u.Id
ORDER BY s.[DateTime] DESC
	
	RETURN
