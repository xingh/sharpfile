USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_InsertGame]    Script Date: 03/11/2007 23:35:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_InsertGame]
(
	@MatchId INT = NULL,
	@Player1 INT = NULL,
	@Player1Points INT = 0,
	@Player2 INT = NULL,
	@Player2Points INT = 0
)
AS
SET NOCOUNT ON

DECLARE @GameId INT;
	
IF @Player1 IS NOT NULL AND @Player2 IS NOT NULL AND @MatchId IS NOT NULL
BEGIN
	BEGIN TRANSACTION t
		INSERT INTO Game (MatchId)
		VALUES (@MatchId);
	
		SET @GameId = @@IDENTITY;
		
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
	
RETURN