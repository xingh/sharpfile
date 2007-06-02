USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SlogGetSlog]    Script Date: 06/02/2007 18:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SlogGetSlog]
	(
	@Id INT
	)
AS
	SET NOCOUNT ON
	
SELECT	s.Id, s.Content, s.[DateTime], u.Id AS 'UserId', u.[Name], s.Title
FROM	Slog s
	JOIN SiteUser u ON s.UserId = u.Id
WHERE	s.Id = @Id;
	
	RETURN
