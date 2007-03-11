USE [GameTracker]
GO
/****** Object:  StoredProcedure [dbo].[usp_InsertTournament]    Script Date: 03/11/2007 23:36:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_InsertTournament] 
(
@Name VARCHAR(100)
)
AS
SET NOCOUNT ON

INSERT INTO Tournament ([Name])
VALUES (@Name);

RETURN