USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_NewsGet]    Script Date: 06/02/2007 18:22:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_NewsGet]
AS
	SET NOCOUNT ON
	
SELECT TOP 10 DateStamp, NewsText
FROM News
ORDER BY DateStamp DESC;
	
	RETURN
