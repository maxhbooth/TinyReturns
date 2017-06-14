/****** Object:  Table [Portfolio].[Countries]    Script Date: 4/15/2017 7:37:24 AM ******/
CREATE TABLE [Portfolio].[Countries](
	[CountryId] [int] NOT NULL,
	[CountryName] [nvarchar](255) NOT NULL,
	[DateCreated] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[DateUpdated] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
 CONSTRAINT [PK_Portfolio_Country] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Portfolio].[Portfolio] ADD [CountryId] [int] NOT NULL DEFAULT 0
GO

INSERT [Portfolio].[Countries] ([CountryId], [CountryName]) VALUES (0, N'None selected')
GO
INSERT [Portfolio].[Countries] ([CountryId], [CountryName]) VALUES (1, N'Australia')
GO
INSERT [Portfolio].[Countries] ([CountryId], [CountryName]) VALUES (2, N'United Kingdom')
GO
INSERT [Portfolio].[Countries] ([CountryId], [CountryName]) VALUES (3, N'United States')
GO


ALTER TABLE [Portfolio].[Portfolio]  WITH CHECK ADD CONSTRAINT [FK_Portfolio_Country] FOREIGN KEY([CountryId])
REFERENCES [Portfolio].[Countries] ([CountryId])
GO

ALTER TABLE [Portfolio].[Portfolio] CHECK CONSTRAINT [FK_Portfolio_Country]
GO

