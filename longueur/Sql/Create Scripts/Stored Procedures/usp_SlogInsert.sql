USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_SlogInsert]    Script Date: 06/02/2007 18:25:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SlogInsert] 
	(
	@Content TEXT,
	@UserId INT,
	@Title VARCHAR(50)
	)
AS
	SET NOCOUNT ON
	
INSERT INTO Slog (Content, UserId, Title)
VALUES	(@Content, @UserId, @Title)
	
	RETURN
