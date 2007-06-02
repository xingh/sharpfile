USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_DownloadGet]    Script Date: 06/02/2007 18:21:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_DownloadGet]
AS
	SET NOCOUNT ON
	
SELECT TOP 100 f.[Filename], d.*
FROM Download d
	JOIN [Filename] f ON d.Id = f.Id
ORDER BY [DateTime] DESC
	
	RETURN