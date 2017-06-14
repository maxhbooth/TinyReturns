using System;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.IntegrationTests.Core;
using Dimensional.TinyReturns.Web.Controllers;
using Dimensional.TinyReturns.Web.Models;
using FluentAssertions;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class TestHelper
    {
        private readonly AllTablesDeleter _allTablesDeleter;
        private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
        private readonly CountriesDataTableGateway _countriesDataTableGateway;
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

            _countriesDataTableGateway = new CountriesDataTableGateway(
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
                _countriesDataTableGateway,
                returnSeriesRepository,
                benchmarkWithPerformanceRepository);

            var publicWebReportFacade = new PublicWebReportFacade(
                portfolioWithPerformanceRepository,
                new ClockStub(CurrentDate));

            return new PortfolioPerformanceController(
                publicWebReportFacade,
                new ClockStub(CurrentDate),
                _countriesDataTableGateway);
        }

        public void InsertPortfolioDto(
            PortfolioDto dto)
        {
            _portfolioDataTableGateway.Insert(dto);
        }

        public void InsertCountryDto(
            CountryDto dto)
        {
            _countriesDataTableGateway.Insert(dto);
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

            ((PortfolioPerformanceIndexModel)viewResult.Model).Portfolios.Should().BeAssignableTo<PublicWebReportFacade.PortfolioModel[]>();
            return ((PortfolioPerformanceIndexModel)viewResult.Model).Portfolios;
        }

        public PortfolioPerformanceIndexModel GetModelFromActionResult(
            ActionResult actionResult)
        {
            actionResult.Should().BeAssignableTo<ViewResult>();
            var viewResult = (ViewResult)actionResult;

            viewResult.Model.Should().BeAssignableTo<PortfolioPerformanceIndexModel>();
            return (PortfolioPerformanceIndexModel)viewResult.Model;
        }

        public PortfolioPerformanceIndexModel GetPortfolioModelFromActionResult(
            ActionResult actionResult)
        {
            actionResult.Should().BeAssignableTo<ViewResult>();
            var viewResult = (ViewResult)actionResult;

            viewResult.Model.Should().BeAssignableTo<PortfolioPerformanceIndexModel>();
            var portfolioPerformanceNetGrossModelModel = (PortfolioPerformanceIndexModel)viewResult.Model;

            return portfolioPerformanceNetGrossModelModel;
        }

        public void AssertSelectItemsArePopulated(
            PortfolioPerformanceIndexModel resultModel)
        {
            resultModel.NetGrossList.Should().NotBeNull();
            resultModel.NetGrossList.Count().Should().Be(2);

            var arrayBinary = resultModel.NetGrossList.ToArray();

            arrayBinary[0].Value.Should().Be("0");
            arrayBinary[0].Text.Should().Be("Net");

            arrayBinary[1].Value.Should().Be("1");
            arrayBinary[1].Text.Should().Be("Gross");

            arrayBinary.All(m => m != null).Should().BeTrue();
        }
        public void AssertSelectItemDefaultsNet(
            PortfolioPerformanceIndexModel resultModel)
        {
            resultModel.Selected.Should().Be("0");
        }

        internal void AssertModelIsNet(PortfolioPerformanceIndexModel model)
        {
            model.Selected.Should().Be("0");
        }

        internal void AssertModelIsGross(PortfolioPerformanceIndexModel model)
        {
            model.Selected.Should().Be("1");
        }
    }

}