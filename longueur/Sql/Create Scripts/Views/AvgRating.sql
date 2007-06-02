USE [Longueur]
GO
/****** Object:  View [dbo].[AvgRating]    Script Date: 06/02/2007 18:19:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AvgRating]
AS
SELECT     q.QuoteID, AVG(CAST(r.Rating AS float)) AS AvgRating
FROM         dbo.Quote q INNER JOIN
                      dbo.Rating r ON q.QuoteID = r.QuoteID
GROUP BY q.QuoteID
HAVING      (COUNT(r.Rating) > 1)
