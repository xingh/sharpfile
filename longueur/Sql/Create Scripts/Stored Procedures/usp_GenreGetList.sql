USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_GenreGetList]    Script Date: 06/02/2007 18:22:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GenreGetList] 
AS
	SET NOCOUNT ON

SELECT GenreID, GenreName
FROM Genre;
	
	RETURN
