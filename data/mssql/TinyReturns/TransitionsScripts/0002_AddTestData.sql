INSERT INTO [EntityType] ([EntityTypeCode],[Name]) VALUES ('P', 'Portfolio')
INSERT INTO [EntityType] ([EntityTypeCode],[Name]) VALUES ('B', 'Benchmark')

-- **********************

INSERT INTO [FeeType] ([FeeTypeCode],[Name]) VALUES ('0', 'Not Applicable')
INSERT INTO [FeeType] ([FeeTypeCode],[Name]) VALUES ('N', 'Net of Fees')
INSERT INTO [FeeType] ([FeeTypeCode],[Name]) VALUES ('G', 'Gross of Fees')

-- **********************

INSERT INTO [Entity] ([EntityNumber], [Name], [EntityTypeCode]) VALUES (100, 'Portfolio 100 - Large', 'P')
INSERT INTO [Entity] ([EntityNumber], [Name], [EntityTypeCode]) VALUES (101, 'Portfolio 101 - Medium', 'P')
INSERT INTO [Entity] ([EntityNumber], [Name], [EntityTypeCode]) VALUES (102, 'Portfolio 102 - Large', 'P')

INSERT INTO [Entity] ([EntityNumber], [Name], [EntityTypeCode]) VALUES (10000, 'Benchmark 00', 'B')
INSERT INTO [Entity] ([EntityNumber], [Name], [EntityTypeCode]) VALUES (10001, 'Benchmark 01', 'B')
