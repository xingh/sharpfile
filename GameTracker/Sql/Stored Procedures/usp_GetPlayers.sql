USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetPlayers]    Script Date: 03/11/2007 23:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetPlayers]
AS
SET NOCOUNT ON
	
SELECT	Id, [Name]
FROM	Player
ORDER BY Id ASC;
	
RETURN
