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
    public class PortfolioPerformanceControllerForGetForIndexTests
    {
        private TestHelper _testhelper = new TestHelper();

        [Fact]
        public void ShouldReturnNoRecordsWhenNoPortfolioAreFound()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
                {
                    var controller = testHelper.CreateController();

                    // Act
                    var actionResult = controller.Index();

                    // Assert
                    var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);
                    var viewResultModel = _testhelper.GetModelFromActionResult(actionResult);

                    viewResultModel.MonthYears.Count().Should().Be(0);
                    viewResultModel.MonthYear.Should().NotBeEmpty();
                    viewResultPortfolio.Length.Should().Be(0);
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
                var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = _testhelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(0); //only care about performance numbers so.
                viewResultModel.MonthYear.Should().NotBeEmpty();

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(100);
                viewResultPortfolio[0].Name.Should().Be("Portfolio 100");
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();
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
                    Name = "Return Series for Portfolio 100"
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
                var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = _testhelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(1);
                viewResultModel.MonthYears.First().Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModel.MonthYears.First().Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(PercentHelper.AsPercent(0.02m), 0.00001m);
                viewResultPortfolio[0].ThreeMonth.Should().NotHaveValue();
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(PercentHelper.AsPercent(0.02m), 0.00001m);
            });
        }


        [Fact]
        public void ShouldReturnSinglePortfolioWithReturnsForfiveMonths()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);
                var monthYearMinus1 = monthYear.AddMonths(-1);
                var monthYearMinus2 = monthYear.AddMonths(-2);
                var monthYearMinus3 = monthYear.AddMonths(-3);
                var monthYearMinus4 = monthYear.AddMonths(-4);
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
                    Name = "Return Series for Portfolio 100"
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

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = _testhelper.GetModelFromActionResult(actionResult);
                var viewResultModelArray = viewResultModel.MonthYears.ToArray();

                viewResultModel.MonthYears.Count().Should().Be(5);

                viewResultModelArray[0].Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[0].Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[1].Value.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[1].Text.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[2].Value.Should()
                    .Be(monthYearMinus2.Month.ToString() + "/" + monthYearMinus2.Year.ToString());
                viewResultModelArray[2].Text.Should()
                    .Be(monthYearMinus2.Month.ToString() + "/" + monthYearMinus2.Year.ToString());

                viewResultModelArray.All(m => m != null).Should().BeTrue();

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(PercentHelper.AsPercent(0.02m), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(PercentHelper.AsPercent(0.039584m), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(PercentHelper.AsPercent(0.0394800416m), 0.00000001m);
            });
        }


        [Fact]
        public void ShouldReturnSinglePortfolioWithReturnsForEightMonths()
        {
            // Arrange
            var testHelper = new TestHelper();
            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 8);
                var monthYearMinus1 = monthYear.AddMonths(-1);
                var monthYearMinus2 = monthYear.AddMonths(-2);
                var monthYearMinus3 = monthYear.AddMonths(-3);
                var monthYearMinus4 = monthYear.AddMonths(-4);
                var monthYearMinus5 = monthYear.AddMonths(-5);
                var monthYearMinus6 = monthYear.AddMonths(-6);
                var monthYearMinus7 = monthYear.AddMonths(-7);
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
                    Year = monthYearMinus7.Year,
                    Month = monthYearMinus7.Month,
                    ReturnValue = 0.01m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus6.Year,
                    Month = monthYearMinus6.Month,
                    ReturnValue = 0.02m
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYearMinus5.Year,
                    Month = monthYearMinus5.Month,
                    ReturnValue = 0.03m
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

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = _testhelper.GetModelFromActionResult(actionResult);
                var viewResultModelArray = viewResultModel.MonthYears.ToArray();

                viewResultModel.MonthYears.Count().Should().Be(8);

                viewResultModelArray[0].Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[0].Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[1].Value.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[1].Text.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[2].Value.Should()
                    .Be(monthYearMinus2.Month.ToString() + "/" + monthYearMinus2.Year.ToString());
                viewResultModelArray[2].Text.Should()
                    .Be(monthYearMinus2.Month.ToString() + "/" + monthYearMinus2.Year.ToString());

                viewResultModelArray.All(m => m != null).Should().BeTrue();

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                var expectedThreeMonthResult = (1.02m) * (.98m) * (1.04m) - 1;
                var expectedSixMonthResult = (1.02m) * (.98m) * (1.04m) * (1.01m) * (.99m) * (1.03m) - 1;
                var expectedQuarterToDateResult = (1.02m) * (.98m) - 1;
                var expectedYearToDateResult = (1.02m) * (.98m) * (1.04m) * (1.01m)
                                                * (.99m) * (1.03m) * (1.02m) * (1.01m)- 1;

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(PercentHelper.AsPercent(0.02m), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedThreeMonthResult), 0.00000001m);
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(
                    PercentHelper.AsPercent(expectedSixMonthResult), 0.00000001m);
		        viewResultPortfolio[0].QuarterToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedQuarterToDateResult), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedYearToDateResult), 0.00000001m);
            });
        }

        [Fact]
        public void ShouldReturnPortfolioWithBenchmark()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var benchmarkNumber = 10000;
                var benchmarkName = "Benchmark 10000";

                var monthYear = new MonthYear(2016, 5);
                var nextMonth = monthYear.AddMonths(1);

                testHelper.CurrentDate = new DateTime(
                    nextMonth.Year,
                    nextMonth.Month,
                    5);

                // **

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var portfolioReturnSeriesId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = portfolioReturnSeriesId,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var portfolioMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    portfolioReturnSeriesId,
                    new MonthYearRange(monthYear.AddMonths(-6), monthYear));// should give 7 months

                foreach (var portfolioMonthlyReturnDto in portfolioMonthlyReturnDtos)
                {
                    Debug.WriteLine(portfolioMonthlyReturnDto.ReturnValue);
                }

                testHelper.InsertMonthlyReturnDtos(portfolioMonthlyReturnDtos);

                // **

                testHelper.InsertBenchmarkDto(new BenchmarkDto()
                {
                    Number = benchmarkNumber,
                    Name = benchmarkName
                });

                var benchmarkReturnSeriesId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Return Series for Benchmark X"
                });

                testHelper.InsertBenchmarkToReturnSeriesDto(new BenchmarkToReturnSeriesDto()
                {
                    BenchmarkNumber = benchmarkNumber,
                    ReturnSeriesId = benchmarkReturnSeriesId
                });

                var benchmarkMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    benchmarkReturnSeriesId,
                    new MonthYearRange(monthYear.AddMonths(-6), monthYear), //should give seven months
                    seed: 8);
                foreach (var benchmarkMonthlyReturnDto in benchmarkMonthlyReturnDtos)
                {
                    Debug.WriteLine(benchmarkMonthlyReturnDto.ReturnValue);
                }
                
                testHelper.InsertMonthlyReturnDtos(benchmarkMonthlyReturnDtos);

                // **

                testHelper.InsertPortfolioToBenchmarkDto(new PortfolioToBenchmarkDto()
                {
                    PortfolioNumber = portfolioNumber,
                    BenchmarkNumber = benchmarkNumber,
                    SortOrder = 1
                });

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultPortfolio = _testhelper.GetPortfoliosFromActionResult(actionResult);

                var expectedViewOneMonth = (1 + 0.812m) - 1;
                var expectedViewThreeMonth = (1 + 0.812m) * (1 + 0.1177m) * (1 -0.588m) - 1;
                var expectedViewSixMonth = (1 + 0.812m) * (1 + 0.1177m) * (1 -0.588m) * (1 + 0.1163m)
                                           * (1 + 0.536m) * (1 + 0.6346m) - 1;
                var expectedViewQuarterToDate= (1 + 0.812m) * (1 + 0.1177m) -1;
                var expectedViewYearToDate = (1 + 0.812m) * (1 + 0.1177m) * (1 -0.588m) * (1 + 0.1163m)
                                             * (1 + 0.536m) - 1;
                // note we only include 5 months because the monthyear actually starts with may.

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedViewOneMonth), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedViewThreeMonth), 0.00000001m);
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedViewSixMonth), 0.00000001m);
                viewResultPortfolio[0].QuarterToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedViewQuarterToDate), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedViewYearToDate), 0.00000001m);

                viewResultPortfolio[0].Benchmarks.Should().HaveCount(1);

                var benchmarkModel = viewResultPortfolio[0].Benchmarks[0];

                var expectedBenchOneMonth = (1 - 0.0191m) - 1;
                var expectedBenchThreeMonth = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) - 1;
                var expectedBenchSixMonth = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) * (1 - 0.4686m)
                                            * (1 - 0.2802m) * (1 - 0.6707m) - 1;
                var expectedBenchQuarterToDate = (1 - 0.0191m) * (1 + .1001m) - 1;
                var expectedBenchYearToDate = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) * (1 - 0.4686m)
                                              * (1 - 0.2802m)  - 1;

                benchmarkModel.Name.Should().Be(benchmarkName);
                benchmarkModel.OneMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchOneMonth), 0.00000001m);
                benchmarkModel.ThreeMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchThreeMonth), 0.00000001m);
                benchmarkModel.SixMonth.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchSixMonth), 0.00000001m);
                benchmarkModel.QuarterToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchQuarterToDate), 0.00000001m);
                benchmarkModel.YearToDate.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchYearToDate), 0.00000001m);
            });
        }


    }
}