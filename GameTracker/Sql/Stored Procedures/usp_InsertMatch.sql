USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_InsertMatch]    Script Date: 03/11/2007 23:36:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_InsertMatch] 
(
	@TournamentId INT = 1,
	@DateTime DATETIME = NULL,
	@Player1 INT = NULL,
	@Player1Points INT = 0,
	@Player2 INT = NULL,
	@Player2Points INT = 0
)
AS
SET NOCOUNT ON

DECLARE @MatchId INT;
DECLARE @GameId INT;

IF @DateTime IS NULL
BEGIN
	SET @DateTime = GETDATE();
END
	
IF @Player1 IS NOT NULL AND @Player2 IS NOT NULL
BEGIN
	BEGIN TRANSACTION t
		INSERT INTO Match (TournamentId, [DateTime])
		VALUES (@TournamentId, @DateTime);
		
		SET @MatchId = SCOPE_IDENTITY();
		
		IF @MatchId IS NULL
		BEGIN
			ROLLBACK TRANSACTION t
		END
		ELSE
		BEGIN
			INSERT INTO Game (MatchId)
			VALUES (@MatchId);
		
			SET @GameId = SCOPE_IDENTITY();
			
			IF @GameId IS NULL
			BEGIN
				ROLLBACK TRANSACTION t
			END
			ELSE
			BEGIN
				INSERT INTO Score (GameId, PlayerId, Points)
				VALUES (@GameId, @Player1, @Player1Points);
				
				INSERT INTO Score (GameId, PlayerId, Points)
				VALUES (@GameId, @Player2, @Player2Points);
				
				COMMIT TRANSACTION t;
			END
		END
END
	
RETURN
