CREATE TABLE [dbo].[InvestmentVehicle](
	[InvestmentVehicleNumber] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[InvestmentVehicleTypeCode] [char](1) NOT NULL,
 CONSTRAINT [PK_InvestmentVehicle] PRIMARY KEY CLUSTERED 
(
	[InvestmentVehicleNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[InvestmentVehicleType](
	[InvestmentVehicleTypeCode] [char](1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_InvestmentVehicleType] PRIMARY KEY CLUSTERED 
(
	[InvestmentVehicleTypeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[FeeType](
	[FeeTypeCode] [char](1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_FeeType] PRIMARY KEY CLUSTERED 
(
	[FeeTypeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[MonthlyReturn](
	[ReturnSeriesId] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[ReturnValue] [decimal](18, 8) NOT NULL,
 CONSTRAINT [PK_Return] PRIMARY KEY CLUSTERED 
(
	[ReturnSeriesId] ASC,
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ReturnSeries](
	[ReturnSeriesId] [int] IDENTITY(1,1) NOT NULL,
	[InvestmentVehicleNumber] [int] NOT NULL,
	[FeeTypeCode] [char](1) NOT NULL,
 CONSTRAINT [PK_ReturnSeries] PRIMARY KEY CLUSTERED 
(
	[ReturnSeriesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvestmentVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InvestmentVehicle_InvestmentVehicleType] FOREIGN KEY([InvestmentVehicleTypeCode])
REFERENCES [dbo].[InvestmentVehicleType] ([InvestmentVehicleTypeCode])
GO

ALTER TABLE [dbo].[InvestmentVehicle] CHECK CONSTRAINT [FK_InvestmentVehicle_InvestmentVehicleType]
GO

ALTER TABLE [dbo].[MonthlyReturn]  WITH CHECK ADD  CONSTRAINT [FK_MonthlyReturn_ReturnSeries] FOREIGN KEY([ReturnSeriesId])
REFERENCES [dbo].[ReturnSeries] ([ReturnSeriesId])
GO

ALTER TABLE [dbo].[MonthlyReturn] CHECK CONSTRAINT [FK_MonthlyReturn_ReturnSeries]
GO

ALTER TABLE [dbo].[ReturnSeries]  WITH CHECK ADD  CONSTRAINT [FK_ReturnSeries_FeeType] FOREIGN KEY([FeeTypeCode])
REFERENCES [dbo].[FeeType] ([FeeTypeCode])
GO

ALTER TABLE [dbo].[ReturnSeries] CHECK CONSTRAINT [FK_ReturnSeries_FeeType]
GO

ALTER TABLE [dbo].[ReturnSeries]  WITH CHECK ADD  CONSTRAINT [FK_ReturnSeries_InvestmentVehicle] FOREIGN KEY([InvestmentVehicleNumber])
REFERENCES [dbo].[InvestmentVehicle] ([InvestmentVehicleNumber])
GO


