USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetMatches]    Script Date: 03/11/2007 23:35:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetMatches]
(
	@TournamentId INT = 1
)
AS
SET NOCOUNT ON
	
SELECT m.Id, [DateTime],
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id ASC) AS 'Player1Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id ASC) AS 'Player1',
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id DESC) AS 'Player2Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id DESC) AS 'Player2'
FROM	Match m
WHERE	TournamentId = @TournamentId
GROUP BY m.Id, m.DateTime
ORDER BY [DateTime] DESC;
	
RETURN