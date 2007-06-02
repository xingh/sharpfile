USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_DownloadInsert]    Script Date: 06/02/2007 18:21:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ahill
-- Create date: 02/23/07
-- Description:	Adds a filename if it isn't already there, and inserts a download.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadInsert]
(
	@Filename varchar(100) = NULL,
	@IP varchar(255) = NULL,
	@Referrer varchar(255) = NULL,
	@UserAgent varchar(255) = NULL,
	@Browser varchar(255) = NULL,
	@Platform varchar(255) = NULL,
	@BrowserVersion varchar(255) = NULL,
	@HostName varchar(255) = NULL
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Id INT;
	SET @Id = (
		SELECT	Id
		FROM	dbo.[Filename] f
		WHERE	f.[Filename] = @Filename)

	IF @Id IS NULL
	BEGIN
		INSERT INTO dbo.[Filename] ([Filename]) VALUES (@Filename);
		
		SET @Id = SCOPE_IDENTITY();
	END

	INSERT INTO dbo.[Download] (Id, IP, Referrer, UserAgent, Browser, Platform, BrowserVersion, HostName)
		VALUES (@Id, @IP, @Referrer, @UserAgent, @Browser, @Platform, @BrowserVersion, @HostName)
END
