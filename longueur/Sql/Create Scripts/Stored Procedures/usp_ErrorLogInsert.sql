USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_ErrorLogInsert]    Script Date: 06/02/2007 18:22:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_ErrorLogInsert]
	(
	@Message VARCHAR(500) = NULL,
	@Stacktrace VARCHAR(1024) = NULL,
	@IP VARCHAR(20) = NULL,
	@Url VARCHAR(200) = NULL
	)
AS
	SET NOCOUNT ON
	
INSERT INTO dbo.ErrorLog (Message, Stacktrace, IP, Url) 
	VALUES (@Message, @Stacktrace, @IP, @Url)
	
	RETURN
