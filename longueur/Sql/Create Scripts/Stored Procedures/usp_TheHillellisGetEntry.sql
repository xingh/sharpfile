USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisGetEntry]    Script Date: 06/02/2007 18:26:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_TheHillellisGetEntry]
	(
	@Id INT
	)
AS
	SET NOCOUNT ON
	
SELECT	h.Id, h.[Content], h.[DateTime], u.Id AS 'UserId', u.[Name], h.Title
FROM	TheHillellis h
	JOIN SiteUser u ON h.UserId = u.Id
WHERE	h.Id = @Id;
	
	RETURN