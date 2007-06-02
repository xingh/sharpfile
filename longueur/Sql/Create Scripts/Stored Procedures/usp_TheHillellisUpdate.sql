USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisUpdate]    Script Date: 06/02/2007 18:26:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_TheHillellisUpdate]
	(
	@Id INT,
	@Title VARCHAR(255),
	@Content TEXT,
	@UserId INT
	)
AS
	SET NOCOUNT ON
	
UPDATE TheHillellis 
SET Title = @Title, 
	Content = @Content, 
	UserId = @UserId 
WHERE Id = @Id;
	
	RETURN
