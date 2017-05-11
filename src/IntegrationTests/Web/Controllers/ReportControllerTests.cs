using System;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.IntegrationTests.Core;
using Dimensional.TinyReturns.Web.Controllers;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class ReportControllerTests
    {
        public class TestHelper
        {
            private readonly AllTablesDeleter _allTablesDeleter;
            private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
            private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                CurrentDate = new DateTime(2017, 2, 3);

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
            }

            public DateTime CurrentDate { get; set; }

            public ReportController CreateController()
            {
                var returnSeriesRepository = new ReturnSeriesRepository(
                    _returnSeriesDataTableGateway,
                    _monthlyReturnDataTableGateway);

                var portfolioWithPerformanceRepository = new PortfolioWithPerformanceRepository(
                    _portfolioDataTableGateway,
                    _portfolioToReturnSeriesDataTableGateway,
                    returnSeriesRepository);

                var publicWebReportFacade = new PublicWebReportFacade(
                    portfolioWithPerformanceRepository, 
                    new ClockStub(CurrentDate));

                return new ReportController(
                    publicWebReportFacade);
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
                _monthlyReturnDataTableGateway.Insert(new []{ dto });
            }

            public void InsertPortfolioToReturnSeriesDto(PortfolioToReturnSeriesDto dto)
            {
                _portfolioToReturnSeriesDataTableGateway.Insert(new []{ dto });
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
        }

        [Fact]
        public void ShouldReturnNoRecrodsWhenNoPortfolioAreFound()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
                {
                    var controller = testHelper.CreateController();

                    // Act
                    var actionResult = controller.Index();

                    // Assert
                    var viewResultModel = GetModelFromActionResult(actionResult);

                    viewResultModel.Length.Should().Be(0);
                });
        }

        [Fact]
        public void ShouldReturnSinglePortfolioWithoutPerformanceNumbers()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "Portfolio 100",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = GetModelFromActionResult(actionResult);

                viewResultModel.Length.Should().Be(1);

                viewResultModel[0].Number.Should().Be(100);
                viewResultModel[0].Name.Should().Be("Portfolio 100");
            });
        }

        [Fact]
        public void ShouldReturnSinglePortfolioSingleNetMonthOfPerformance()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2017, 1);
                var nextMonth = monthYear.AddMonths(1);

                testHelper.CurrentDate = new DateTime(
                    nextMonth.Year,
                    nextMonth.Month,
                    5);

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
                    Year = monthYear.Year,
                    Month = monthYear.Month,
                    ReturnValue = 0.02m
                });

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = GetModelFromActionResult(actionResult);

                viewResultModel.Length.Should().Be(1);

                viewResultModel[0].Number.Should().Be(portfolioNumber);
                viewResultModel[0].Name.Should().Be(portfolioName);
                viewResultModel[0].OneMonth.Should().BeApproximately(0.02m, 0.00001m);
            });
        }

        private static PublicWebReportFacade.PortfolioModel[] GetModelFromActionResult(
            ActionResult actionResult)
        {
            actionResult.Should().BeAssignableTo<ViewResult>();
            var viewResult = (ViewResult) actionResult;

            viewResult.Model.Should().BeAssignableTo<PublicWebReportFacade.PortfolioModel[]>();
            return (PublicWebReportFacade.PortfolioModel[]) viewResult.Model;
        }
    }
}