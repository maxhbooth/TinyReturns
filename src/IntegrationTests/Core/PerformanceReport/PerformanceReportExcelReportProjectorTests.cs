using System;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Dimensional.TinyReturns.Core.PublicWebReport;
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