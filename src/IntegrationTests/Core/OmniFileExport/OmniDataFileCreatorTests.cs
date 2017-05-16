using System;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.OmniFileExport
{
    public class OmniDataFileCreatorTests
    {
        public class TestHelper
        {
            private readonly AllTablesDeleter _allTablesDeleter;
            private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
            private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
            private readonly FlatFileIoSpy _flatFileIoSpy;
            private readonly PortfolioToBenchmarkDataTableGateway _portfolioToBenchmarkDataTableGateway;
            private readonly BenchmarkDataTableGateway _benchmarkDataTableGateway;
            private readonly BenchmarkToReturnSeriesDataTableGateway _benchmarkToReturnSeriesDataTableGateway;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                _allTablesDeleter = new AllTablesDeleter();

                _portfolioDataTableGateway = new PortfolioDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _monthlyReturnDataTableGateway = new MonthlyReturnDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioToReturnSeriesDataTableGateway = new PortfolioToReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioToBenchmarkDataTableGateway = new PortfolioToBenchmarkDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _benchmarkDataTableGateway = new BenchmarkDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _benchmarkToReturnSeriesDataTableGateway = new BenchmarkToReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);
                
                _flatFileIoSpy = new FlatFileIoSpy();
            }

            public FlatFileIoSpy FlatFileIoSpy
            {
                get { return _flatFileIoSpy; }
            }

            public void InsertPortfolioDto(
                PortfolioDto dto)
            {
                _portfolioDataTableGateway.Insert(dto);
            }

            public int InsertReturnSeriesDto(ReturnSeriesDto dto)
            {
                return _returnSeriesDataTableGateway.Insert(dto);
            }

            public void InsertMonthlyReturnDtos(MonthlyReturnDto[] dtos)
            {
                _monthlyReturnDataTableGateway.Insert(dtos);
            }

            public void InsertPortfolioToReturnSeriesDto(PortfolioToReturnSeriesDto dto)
            {
                _portfolioToReturnSeriesDataTableGateway.Insert(new[] { dto });
            }

            public void DatabaseDataDeleter(
                Action act)
            {
                var databaseSettings = new DatabaseSettings();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);

                act();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);
            }


            public OmniDataFileCreator CreateCreator()
            {
                var returnSeriesRepository = new ReturnSeriesRepository(
                    _returnSeriesDataTableGateway,
                    _monthlyReturnDataTableGateway);

                var benchmarkWithPerformanceRepository = new BenchmarkWithPerformanceRepository(
                    _benchmarkDataTableGateway,
                    _benchmarkToReturnSeriesDataTableGateway,
                    returnSeriesRepository);

                var portfolioWithPerformanceRepository = new PortfolioWithPerformanceRepository(
                    _portfolioDataTableGateway,
                    _portfolioToReturnSeriesDataTableGateway,
                    _portfolioToBenchmarkDataTableGateway,
                    returnSeriesRepository,
                    benchmarkWithPerformanceRepository);

                return new OmniDataFileCreator(
                    portfolioWithPerformanceRepository,
                    _flatFileIoSpy);
            }
        }

        [Fact]
        public void ShouldRenderEmptyFileWhenNoPortfoliosExist()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var omniDataFileCreator = testHelper.CreateCreator();

                omniDataFileCreator.CreateFile(new MonthYear(2010, 1), "c:\\temp\\Export.txt");

                var flatFileIoSpy = testHelper.FlatFileIoSpy;

                flatFileIoSpy.NumberOfLines.Should().Be(1);
                flatFileIoSpy.FirstLine().Should().Be("Fund Id|Name|Fee Type|Duration|End Date|Return Value");
            });
        }

        [Fact]
        public void ShouldReturnExpectedDataOnIncompleteYear()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 8);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100",
                    Disclosure = string.Empty
                });

                var returnSeriesIdGross = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100",
                    Disclosure = string.Empty
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-7),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdGross,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.GrossSeriesTypeCode
                });

                var grossMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdGross,
                    monthYearRange,
                    seed: 2);

                testHelper.InsertMonthlyReturnDtos(grossMonthlyReturnDtos);

                var omniDataFileCreator = testHelper.CreateCreator();

                omniDataFileCreator.CreateFile(monthYear, "c:\\temp\\Export.txt");

                var flatFileIoSpy = testHelper.FlatFileIoSpy;

                flatFileIoSpy.NumberOfLines.Should().Be(23);
                flatFileIoSpy.FirstLine().Should().Be("Fund Id|Name|Fee Type|Duration|End Date|Return Value");

                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-1-31|0.54210000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-2-29|-0.19170000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-3-31|-0.66810000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-4-30|0.97000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-5-31|-0.78200000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-6-30|-0.38670000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-7-31|0.60420000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-8-31|-0.10900000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-1-31|0.45240000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-2-29|0.63460000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-3-31|0.53600000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-4-30|0.11630000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-5-31|-0.58800000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-6-30|0.11770000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-7-31|0.81200000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-8-31|-0.11570000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|Q|2016-3-31|-0.586293477183000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|Q|2016-6-30|-0.736612182000000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|Q|2016-3-31|2.646606909440000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|Q|2016-6-30|-0.485952333880000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|Y|2016-8-31|-0.8442513579248284826879466868").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|Y|2016-8-31|2.0036557778971293421545999565").Should().BeTrue();
            });
        }

        [Fact]
        public void ShouldRenderEmptyFileWhenNoPortfoliosExist2()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100",
                    Disclosure = string.Empty
                });

                var returnSeriesIdGross = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100",
                    Disclosure = string.Empty
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdGross,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.GrossSeriesTypeCode
                });

                var grossMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdGross,
                    monthYearRange,
                    seed: 2);

                testHelper.InsertMonthlyReturnDtos(grossMonthlyReturnDtos);

                var omniDataFileCreator = testHelper.CreateCreator();

                omniDataFileCreator.CreateFile(monthYear, "c:\\temp\\Export.txt");

                var flatFileIoSpy = testHelper.FlatFileIoSpy;

                flatFileIoSpy.NumberOfLines.Should().Be(15);
                flatFileIoSpy.FirstLine().Should().Be("Fund Id|Name|Fee Type|Duration|End Date|Return Value");

                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-1-31|0.60420000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-2-29|-0.10900000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-3-31|-0.55010000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-4-30|-0.97760000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|M|2016-5-31|0.53070000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-1-31|0.81200000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-2-29|-0.11570000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-3-31|0.95500000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-4-30|-0.45260000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|M|2016-5-31|-0.41620000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|Q|2016-3-31|-0.356938944220000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|Q|2016-3-31|2.132597378000000000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|G|Y|2016-5-31|-0.9779509282989532096000000000").Should().BeTrue();
                flatFileIoSpy.ContainsLine("100|Portfolio 100|N|Y|2016-5-31|0.0010907851939013600000000000").Should().BeTrue();

            });
        }

    }
}