USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_ErrorLogGet]    Script Date: 06/02/2007 18:22:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_ErrorLogGet] 
AS
	SET NOCOUNT ON
	
SELECT TOP 100 * 
FROM ErrorLog
ORDER BY [DateTime] DESC;
	
	RETURN
