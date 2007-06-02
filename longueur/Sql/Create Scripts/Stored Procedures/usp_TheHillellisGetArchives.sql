USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisGetArchives]    Script Date: 06/02/2007 18:25:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetArchives]
	@Name VARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON

IF @Name IS NOT NULL
BEGIN
	SELECT	a.Id, a.StartDate, a.EndDate, s.[Name], 
		(SELECT COUNT(EntryId)
		FROM TheHillellis_Archive ha
		WHERE ha.ArchiveId = a.Id) AS 'Count'
	FROM	Archive a
	JOIN	SiteUser s ON a.UserId = s.Id
	WHERE	s.[Name] = @Name
	ORDER BY	a.EndDate DESC
END
ELSE 
BEGIN
	SELECT	a.Id, a.StartDate, a.EndDate, s.[Name],
		(SELECT COUNT(EntryId)
		FROM TheHillellis_Archive ha
		WHERE ha.ArchiveId = a.Id) AS 'Count'
	FROM	Archive a
	JOIN	SiteUser s ON a.UserId = s.Id
	ORDER BY	a.EndDate DESC
END
    
END
