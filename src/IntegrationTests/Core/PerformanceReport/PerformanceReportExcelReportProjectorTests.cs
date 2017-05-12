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
        public void ShouldPrintNetOnlyRecordForPortrfolio()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var projector = testHelper.CreateProjector();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

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

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var monthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesId,
                    monthYearRange);

                testHelper.InsertMonthlyReturnDtos(monthlyReturnDtos);

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

                recordModel.OneMonth.Should().BeApproximately(-0.4162m, 0.00000001m);
                recordModel.ThreeMonths.Should().BeApproximately(-0.375236505m, 0.00000001m);
                recordModel.TwelveMonths.Should().NotHaveValue();
                recordModel.YearToDate.Should().BeApproximately(0.0010907851939m, 0.00000001m);

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