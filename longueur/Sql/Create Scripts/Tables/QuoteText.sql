USE [Longueur]
GO
/****** Object:  Table [dbo].[QuoteText]    Script Date: 06/02/2007 18:17:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuoteText](
	[QuoteTextID] [int] IDENTITY(1,1) NOT NULL,
	[QuoteText] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[QuoteID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[DateTimeStamp] [datetime] NOT NULL CONSTRAINT [DF_QuoteText_DateTimeStamp]  DEFAULT (getdate()),
 CONSTRAINT [PK_QuoteText] PRIMARY KEY CLUSTERED 
(
	[QuoteTextID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
