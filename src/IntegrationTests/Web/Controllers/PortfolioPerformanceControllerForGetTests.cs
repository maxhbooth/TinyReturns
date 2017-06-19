﻿using System;
using System.Diagnostics;
using System.Linq;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.IntegrationTests.Core;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class PortfolioPerformanceControllerForGetTests
    {
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
                    var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                    var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);

                    viewResultModel.MonthYears.Count().Should().Be(37);
                    viewResultModel.MonthYear.Should().Be("1/2017");
                    viewResultPortfolio.Length.Should().Be(0);
                    
                    var resultModel = testHelper.GetModelFromActionResult(actionResult);

                    testHelper.AssertSelectItemsArePopulated(resultModel);
                    testHelper.AssertSelectItemDefaultsNet(resultModel);
                });
        }

        [Fact]
        public void ShouldReturnSinglePortfolioWithoutPerformanceNumbers()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var monthYear = new MonthYear(testHelper.CurrentDate);
                var prevMonthYear = monthYear.AddMonths(-1);

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(37);
                viewResultModel.MonthYear.Should().Be(prevMonthYear.Stringify());

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(100);
                viewResultPortfolio[0].Name.Should().Be("Portfolio 100");
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);
                
                viewResultModel.MonthYears.Count().Should().Be(37);
                viewResultModel.MonthYears.First().Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModel.MonthYears.First().Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(0.02m.AsPercent(), 0.00001m);
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();
                viewResultPortfolio[0].StandardDeviation.Should().NotHaveValue();

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(0.02m.AsPercent(), 0.00001m);
                viewResultPortfolio[0].ThreeMonth.Should().NotHaveValue();
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(0.02m.AsPercent(), 0.00001m);
                
                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);
                var viewResultModelArray = viewResultModel.MonthYears.ToArray();

                viewResultModel.MonthYears.Count().Should().Be(37);

                viewResultModelArray[0].Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[0].Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[1].Value.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[1].Text.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());

                viewResultModelArray.All(m => m != null).Should().BeTrue();

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();
                viewResultPortfolio[0].StandardDeviation.Should().NotHaveValue();
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(0.02m.AsPercent(), 0.00000001m);
                
                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(0.039584m.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(0.0394800416m.AsPercent(), 0.00000001m);
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);
                var viewResultModelArray = viewResultModel.MonthYears.ToArray();

                viewResultModel.MonthYears.Count().Should().Be(37);

                viewResultModelArray[0].Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[0].Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[1].Value.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[1].Text.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());

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

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(0.02m.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(expectedThreeMonthResult.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(expectedSixMonthResult.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(expectedSixMonthResult.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].QuarterToDate.Should().BeApproximately(expectedQuarterToDateResult.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(expectedYearToDateResult.AsPercent(), 0.00000001m);

                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);

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

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(expectedViewOneMonth.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(expectedViewThreeMonth.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(expectedViewSixMonth.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].QuarterToDate.Should().BeApproximately(expectedViewQuarterToDate.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(expectedViewYearToDate.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].FirstFullMonth.Should().NotHaveValue();

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
                benchmarkModel.OneMonth.Should().BeApproximately(expectedBenchOneMonth.AsPercent(), 0.00000001m);
                benchmarkModel.ThreeMonth.Should().BeApproximately(expectedBenchThreeMonth.AsPercent(), 0.00000001m);
                benchmarkModel.SixMonth.Should().BeApproximately(expectedBenchSixMonth.AsPercent(), 0.00000001m);
                benchmarkModel.QuarterToDate.Should().BeApproximately(expectedBenchQuarterToDate.AsPercent(), 0.00000001m);
                benchmarkModel.YearToDate.Should().BeApproximately(expectedBenchYearToDate.AsPercent(), 0.00000001m);
		        benchmarkModel.FirstFullMonth.Should().NotHaveValue();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);                
            });
        }

        [Fact]
        public void ShouldReturnPortfolioWithBenchmarkWithFirstFullMonth()
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2016, 1, 1)
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
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult).Portfolios;

                var expectedViewOneMonth = (1 + 0.812m) - 1;
                var expectedViewThreeMonth = (1 + 0.812m) * (1 + 0.1177m) * (1 - 0.588m) - 1;
                var expectedViewSixMonth = (1 + 0.812m) * (1 + 0.1177m) * (1 - 0.588m) * (1 + 0.1163m)
                                           * (1 + 0.536m) * (1 + 0.6346m) - 1;
                var expectedViewQuarterToDate = (1 + 0.812m) * (1 + 0.1177m) - 1;
                var expectedViewYearToDate = (1 + 0.812m) * (1 + 0.1177m) * (1 - 0.588m) * (1 + 0.1163m)
                                             * (1 + 0.536m) - 1;
                // note we only include 5 months because the monthyear actually starts with may.

                var expectedViewFirstFullMonth = (1 + 0.812m) * (1 + 0.1177m) * (1 - 0.588m) * (1 + 0.1163m)
                                                 - 1;
                //FirstFullMonth goes from may until the month after the inception date, which is 4 months.
                viewResultModel.Length.Should().Be(1);

                viewResultModel[0].Number.Should().Be(portfolioNumber);
                viewResultModel[0].Name.Should().Be(portfolioName);

                viewResultModel[0].OneMonth.Should().BeApproximately(expectedViewOneMonth.AsPercent(), 0.00000001m);
                viewResultModel[0].ThreeMonth.Should().BeApproximately(expectedViewThreeMonth.AsPercent(), 0.00000001m);
                viewResultModel[0].SixMonth.Should().BeApproximately(expectedViewSixMonth.AsPercent(), 0.00000001m);
                viewResultModel[0].QuarterToDate.Should().BeApproximately(expectedViewQuarterToDate.AsPercent(), 0.00000001m);
                viewResultModel[0].YearToDate.Should().BeApproximately(expectedViewYearToDate.AsPercent(), 0.00000001m);
                viewResultModel[0].FirstFullMonth.Should().BeApproximately(expectedViewFirstFullMonth.AsPercent(), 0.00000001m);

                viewResultModel[0].Benchmarks.Should().HaveCount(1);

                var benchmarkModel = viewResultModel[0].Benchmarks[0];

                var expectedBenchOneMonth = (1 - 0.0191m) - 1;
                var expectedBenchThreeMonth = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) - 1;
                var expectedBenchSixMonth = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) * (1 - 0.4686m)
                                            * (1 - 0.2802m) * (1 - 0.6707m) - 1;
                var expectedBenchQuarterToDate = (1 - 0.0191m) * (1 + .1001m) - 1;
                var expectedBenchYearToDate = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) * (1 - 0.4686m)
                                              * (1 - 0.2802m) - 1;
                var expectedBenchFirstFullMonth = (1 - 0.0191m) * (1 + .1001m) * (1 + 0.6358m) * (1 - 0.4686m)
                                                  - 1;

                benchmarkModel.Name.Should().Be(benchmarkName);
                benchmarkModel.OneMonth.Should().BeApproximately(expectedBenchOneMonth.AsPercent(), 0.00000001m);
                benchmarkModel.ThreeMonth.Should().BeApproximately(expectedBenchThreeMonth.AsPercent(), 0.00000001m);
                benchmarkModel.SixMonth.Should().BeApproximately(expectedBenchSixMonth.AsPercent(), 0.00000001m);
                benchmarkModel.QuarterToDate.Should().BeApproximately(expectedBenchQuarterToDate.AsPercent(), 0.00000001m);
                benchmarkModel.YearToDate.Should().BeApproximately(expectedBenchYearToDate.AsPercent(), 0.00000001m);
                benchmarkModel.FirstFullMonth.Should().BeApproximately(expectedBenchFirstFullMonth.AsPercent(), 0.00000001m);

            });
        }

        [Fact]
        public void ShouldReturnPortfolioWithBenchmarkForAYear()
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

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
                    new MonthYearRange(monthYear.AddMonths(-11), monthYear)); // should give 12 months

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
                    new MonthYearRange(monthYear.AddMonths(-11), monthYear), //should give 12 months
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
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult).Portfolios;

                var viewValues = new decimal[]
                {
                    0.4524m, 0.6346m, 0.536m, 0.1163m, -0.588m, 0.1177m, 0.812m, -0.1157m, 0.955m, -0.4526m, -0.4162m, -
                        0.0654m
                };

                var viewMean = viewValues.Sum() / viewValues.Length;

                for (int i = 0; i < viewValues.Length; i++)
                {
                    viewValues[i] = (viewMean - viewValues[i]) * (viewMean - viewValues[i]);
                }

                var expectedViewStandardDevation = (Decimal)Math.Sqrt((Double)viewValues.Sum() / viewValues.Length);

                viewResultModel.Length.Should().Be(1);

                viewResultModel[0].Number.Should().Be(portfolioNumber);
                viewResultModel[0].Name.Should().Be(portfolioName);

                viewResultModel[0].StandardDeviation.Should()
                    .BeApproximately(expectedViewStandardDevation.AsPercent(), 0.00000001m);

                viewResultModel[0].Mean.Should()
                    .BeApproximately(viewMean.AsPercent(), 0.00000001m);

                viewResultModel[0].Benchmarks.Should().HaveCount(1);

//                var benchmarkModel = viewResultModel[0].Benchmarks[0];
//
//                var benchValues = new decimal[]
//                {
//                    0.426m, -0.6285m, -0.5526m, 0.9345m, -0.0887m, -0.0191m,
//                    0.1001m, 0.6358m, -0.4686m, -0.2802m, -0.6707m, 0.8112m
//                };
//
//                var benchMean = benchValues.Sum() / benchValues.Length;
//
//                for (int i = 0; i < benchValues.Length; i++)
//                {
//                    benchValues[i] = (benchMean - benchValues[i]) * (benchMean - benchValues[i]);
//                }
//
//                var expectedBenchStandardDevation = (Decimal)Math.Sqrt((Double)benchValues.Sum() / benchValues.Length);
//
//                benchmarkModel.StandardDeviation.Should().BeApproximately(PercentHelper.AsPercent(expectedBenchStandardDevation), 0.00000001m);
//                benchmarkModel.Mean.Should().BeApproximately(PercentHelper.AsPercent(benchMean), 0.00000001m);

            });
        }
        [Fact]
        public void ShouldOnlyReturnStandardDeviationForMonthsSinceInception()
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "Westeros"
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2015, 6, 1)
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
                    new MonthYearRange(monthYear.AddMonths(-35), monthYear)); 

                foreach (var portfolioMonthlyReturnDto in portfolioMonthlyReturnDtos)
                {
                    Debug.WriteLine(portfolioMonthlyReturnDto.ReturnValue + " and Montheyar " + portfolioMonthlyReturnDto.Month + "/" + portfolioMonthlyReturnDto.Year);
                }

                Array.Reverse(portfolioMonthlyReturnDtos);

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
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult).Portfolios;

                var viewValues = new decimal[]
                {
                    0.39980000m, 0.05250000m, 0.86800000m, 0.37520000m, 0.09360000m, -0.83780000m,
                    -0.62580000m, -0.09340000m, -0.40570000m, 0.97700000m, 0.28530000m, 0.52590000m
                };

                var viewMean = viewValues.Sum() / viewValues.Length;

                for (int i = 0; i < viewValues.Length; i++)
                {
                    viewValues[i] = (viewMean - viewValues[i]) * (viewMean - viewValues[i]);
                }

                var expectedViewStandardDevation = (Decimal)Math.Sqrt((Double)viewValues.Sum() / viewValues.Length);

                viewResultModel.Length.Should().Be(1);

                viewResultModel[0].Number.Should().Be(portfolioNumber);
                viewResultModel[0].Name.Should().Be(portfolioName);

                viewResultModel[0].Mean.Should()
                    .BeApproximately(viewMean.AsPercent(), 0.00000001m);

                viewResultModel[0].StandardDeviation.Should()
                    .BeApproximately(expectedViewStandardDevation.AsPercent(), 0.00000001m);



            });
        }
    }
}