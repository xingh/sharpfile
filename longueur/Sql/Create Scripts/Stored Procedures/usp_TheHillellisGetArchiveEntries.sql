USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisGetArchiveEntries]    Script Date: 06/02/2007 18:25:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetArchiveEntries]
	@ArchiveId INT
AS
BEGIN
	SET NOCOUNT ON

SELECT	h.Id, h.[Content], h.[DateTime], h.UserId, h.Title, s.[Name]
FROM	Archive a
JOIN	TheHillellis_Archive ha ON a.Id = ha.ArchiveId
JOIN	TheHillellis h ON ha.EntryId = h.Id
JOIN	SiteUser s ON h.UserId = s.Id
WHERE	a.Id = @ArchiveId
ORDER BY	h.[DateTime] DESC
    
END