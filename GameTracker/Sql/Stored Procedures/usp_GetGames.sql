USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetGames]    Script Date: 03/11/2007 23:35:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetGames]
(
	@MatchId INT = NULL
)
AS
SET NOCOUNT ON
	
SELECT	Id,
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS 'Player1Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS 'Player1',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId ASC) AS 'Player1Points',
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS 'Player2Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS 'Player2',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId DESC) AS 'Player2Points'
FROM	Game g 
WHERE	MatchId = @MatchId
ORDER BY g.Id ASC;
	
RETURN