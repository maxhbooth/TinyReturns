using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.IntegrationTests.Core;
using Dimensional.TinyReturns.Web.Controllers;
using Dimensional.TinyReturns.Web.Models;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class TestHelper
    {
        private readonly AllTablesDeleter _allTablesDeleter;
        private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
        private readonly BenchmarkDataTableGateway _benchmarkDataTableGateway;
        private readonly BenchmarkToReturnSeriesDataTableGateway _benchmarkToReturnSeriesDataTableGateway;
        private readonly PortfolioToBenchmarkDataTableGateway _portfolioToBenchmarkDataTableGateway;

        public TestHelper()
        {
            var databaseSettings = new DatabaseSettings();
            var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

            CurrentDate = new DateTime(2017, 2, 3);

            _allTablesDeleter = new AllTablesDeleter();

            _portfolioDataTableGateway = new PortfolioDataTableGateway(
                databaseSettings,
                systemLogForIntegrationTests);

            _benchmarkDataTableGateway = new BenchmarkDataTableGateway(
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

            _benchmarkToReturnSeriesDataTableGateway = new BenchmarkToReturnSeriesDataTableGateway(
                databaseSettings,
                systemLogForIntegrationTests);

            _portfolioToBenchmarkDataTableGateway = new PortfolioToBenchmarkDataTableGateway(
                databaseSettings,
                systemLogForIntegrationTests);
        }

        public DateTime CurrentDate { get; set; }

        public PortfolioPerformanceController CreateController()
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

            var publicWebReportFacade = new PublicWebReportFacade(
                portfolioWithPerformanceRepository,
                new ClockStub(CurrentDate));

            return new PortfolioPerformanceController(
                publicWebReportFacade, _monthlyReturnDataTableGateway);
        }

        public void InsertPortfolioDto(
            PortfolioDto dto)
        {
            _portfolioDataTableGateway.Insert(dto);
        }

        public void InsertBenchmarkDto(
            BenchmarkDto dto)
        {
            _benchmarkDataTableGateway.Insert(new[] { dto });
        }

        public int InsertReturnSeriesDto(ReturnSeriesDto dto)
        {
            return _returnSeriesDataTableGateway.Insert(dto);
        }

        public void InsertMonthlyReturnDto(MonthlyReturnDto dto)
        {
            _monthlyReturnDataTableGateway.Insert(new[] { dto });
        }

        public void InsertMonthlyReturnDtos(MonthlyReturnDto[] dtos)
        {
            _monthlyReturnDataTableGateway.Insert(dtos);
        }

        public void InsertPortfolioToReturnSeriesDto(PortfolioToReturnSeriesDto dto)
        {
            _portfolioToReturnSeriesDataTableGateway.Insert(new[] { dto });
        }

        public void InsertBenchmarkToReturnSeriesDto(BenchmarkToReturnSeriesDto dto)
        {
            _benchmarkToReturnSeriesDataTableGateway.Insert(new[] { dto });
        }

        public void InsertPortfolioToBenchmarkDto(
            PortfolioToBenchmarkDto dto)
        {
            _portfolioToBenchmarkDataTableGateway.Insert(new[] { dto });
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

        public PublicWebReportFacade.PortfolioModel[] GetPortfoliosFromActionResult(
            ActionResult actionResult)
        {
            actionResult.Should().BeAssignableTo<ViewResult>();
            var viewResult = (ViewResult)actionResult;

            ((PastMonthsModel)viewResult.Model).Portfolios.Should().BeAssignableTo<PublicWebReportFacade.PortfolioModel[]>();
            return ((PastMonthsModel)viewResult.Model).Portfolios;
        }

        public PastMonthsModel GetModelFromActionResult(
            ActionResult actionResult)
        {
            actionResult.Should().BeAssignableTo<ViewResult>();
            var viewResult = (ViewResult)actionResult;

            viewResult.Model.Should().BeAssignableTo<PastMonthsModel>();
            return (PastMonthsModel)viewResult.Model;
        }
    }

}