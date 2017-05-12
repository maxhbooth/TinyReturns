using System;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.PerformanceReport
{
    public class PerformanceReportExcelReportProjectorTests
    {
        public class TestHelper
        {
            private readonly AllTablesDeleter _allTablesDeleter;
            private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
            private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
            private readonly PerformanceReportExcelReportViewSpy _performanceReportExcelReportViewSpy;

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

                _performanceReportExcelReportViewSpy = new PerformanceReportExcelReportViewSpy();
            }

            public PerformanceReportExcelReportViewSpy PerformanceReportExcelReportViewSpy
            {
                get { return _performanceReportExcelReportViewSpy; }
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

            public void InsertMonthlyReturnDto(MonthlyReturnDto dto)
            {
                _monthlyReturnDataTableGateway.Insert(new[] { dto });
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

            public PerformanceReportExcelReportProjector CreateProjector()
            {
                var returnSeriesRepository = new ReturnSeriesRepository(
                    _returnSeriesDataTableGateway,
                    _monthlyReturnDataTableGateway);

                var portfolioWithPerformanceRepository = new PortfolioWithPerformanceRepository(
                    _portfolioDataTableGateway,
                    _portfolioToReturnSeriesDataTableGateway,
                    returnSeriesRepository);

                return new PerformanceReportExcelReportProjector(
                    portfolioWithPerformanceRepository,
                    _performanceReportExcelReportViewSpy);
            }
        }

        [Fact]
        public void ShouldPrintNoModelsWhenNoPortfoliosExists()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var projector = testHelper.CreateProjector();

                projector.CreateReport(
                    new MonthYear(2010, 1),
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();
                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(0);
                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 1/2010");
            });
        }

        [Fact]
        public void ShouldPrintNetRecordForPortrfolio()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var projector = testHelper.CreateProjector();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);
                var monthYearMinus1 = monthYear.AddMonths(-1);
                var monthYearMinus2 = monthYear.AddMonths(-2);
                var monthYearMinus3 = monthYear.AddMonths(-3);
                var monthYearMinus4 = monthYear.AddMonths(-4);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var returnSeriesId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Return Series for Portfolio 100",
                    Disclosure = string.Empty
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesId,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus4.Year,
                    Month = monthYearMinus4.Month,
                    ReturnValue = -0.01m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus3.Year,
                    Month = monthYearMinus3.Month,
                    ReturnValue = 0.01m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus2.Year,
                    Month = monthYearMinus2.Month,
                    ReturnValue = 0.04m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus1.Year,
                    Month = monthYearMinus1.Month,
                    ReturnValue = -0.02m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYear.Year,
                    Month = monthYear.Month,
                    ReturnValue = 0.02m
                });

                projector.CreateReport(
                    monthYear,
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();

                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(1);

                var recordModel = viewSpy.PerformanceReportExcelReportModel.Records[0];

                recordModel.EntityNumber.Should().Be(portfolioNumber);
                recordModel.Name.Should().Be(portfolioName);
                recordModel.Type.Should().Be("Portfolio");
                recordModel.FeeType.Should().Be("Net");

                recordModel.OneMonth.Should().BeApproximately(0.02m, 0.00000001m);
                recordModel.ThreeMonths.Should().BeApproximately(0.039584m, 0.00000001m);
                recordModel.YearToDate.Should().BeApproximately(0.0394800416m, 0.00000001m);

                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 5/2016");
            });
        }

        public class PerformanceReportExcelReportViewSpy : IPerformanceReportExcelReportView
        {
            public bool RenderReportWasCalled { get; private set; }
            public PerformanceReportExcelReportModel PerformanceReportExcelReportModel { get; private set; }
            public string FullFilePath { get; private set; }

            public void RenderReport(
                PerformanceReportExcelReportModel model,
                string fullFilePath)
            {
                FullFilePath = fullFilePath;
                PerformanceReportExcelReportModel = model;
                RenderReportWasCalled = true;
            }
        }
    }
}