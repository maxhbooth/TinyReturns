using System;
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
    public class PortfolioPerformanceControllerForGetForIndexTests
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
                    var viewResultModel = testHelper.GetModelFromActionResult(actionResult);
                    var countryArray = viewResultModel.Countries.ToArray();

                    viewResultModel.MonthYears.Count().Should().Be(37);
                    viewResultModel.MonthYear.Should().NotBeEmpty();
                    viewResultPortfolio.Length.Should().Be(0);

                    countryArray.Length.Should().Be(1);
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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(37); //only care about performance numbers so.
                viewResultModel.MonthYear.Should().NotBeEmpty();

                viewResultPortfolio.Length.Should().Be(1);


                viewResultModel.Portfolios[0].NetGrowthOfWealth.Should().BeNull();
                viewResultModel.Portfolios[0].GrossGrowthOfWealth.Should().BeNull();

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

                var monthYear = new MonthYear(2017, 6);
                var nextMonth = monthYear.AddMonths(1);

                testHelper.CurrentDate = new DateTime(
                    monthYear.Year,
                    nextMonth.Month,
                    5);

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "None SelectedTypeOfReturn"
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1),
                    CountryId = 0
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
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(37);
                viewResultModel.MonthYears.First().Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModel.MonthYears.First().Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());

                viewResultPortfolio.Length.Should().Be(1);

                viewResultPortfolio[0].Number.Should().Be(portfolioNumber);
                viewResultPortfolio[0].Name.Should().Be(portfolioName);
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(0.02m.AsPercent(), 0.00001m);

                viewResultModel.Portfolios[0].NetGrowthOfWealth.Should().NotBeNull();
                viewResultModel.Portfolios[0].GrossGrowthOfWealth.Should().BeNull();

                viewResultModel.Portfolios[0].NetGrowthOfWealth.MonthlyGrowthOfWealthReturn[0].Value.Should().Be(0.02m);

                viewResultPortfolio[0].ThreeMonth.Should().NotHaveValue();
                viewResultPortfolio[0].YearToDate.Should().NotHaveValue();

                viewResultPortfolio[0].Country.Should().Be("None SelectedTypeOfReturn");
                
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

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "None Selected"
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1),
                    CountryId = 0
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

                viewResultPortfolio[0].OneMonth.Should().BeApproximately(0.02m.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].ThreeMonth.Should().BeApproximately(0.039584m.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(0.0394800416m.AsPercent(), 0.00000001m);

                viewResultModel.Portfolios[0].NetGrowthOfWealth.Should().NotBeNull();
                viewResultModel.Portfolios[0].GrossGrowthOfWealth.Should().BeNull();

                var expectedGrowthofWealth1 = (1 - 0.01m) * (1 + 0.01m) - 1;
                var expectedGrowthofWealth2 = (1 - 0.01m) * (1 + 0.01m) * (1+0.04m) - 1;
                var expectedGrowthofWealth3 = (1 - 0.01m) * (1 + 0.01m) * (1 + 0.04m) * (1-0.02m) - 1;

                viewResultModel.Portfolios[0].NetGrowthOfWealth.MonthlyGrowthOfWealthReturn[0].Value.Should().Be(-0.01m);
                viewResultModel.Portfolios[0].NetGrowthOfWealth.MonthlyGrowthOfWealthReturn[1].Value.Should().Be(expectedGrowthofWealth1);
                viewResultModel.Portfolios[0].NetGrowthOfWealth.MonthlyGrowthOfWealthReturn[2].Value.Should().Be(expectedGrowthofWealth2);
                viewResultModel.Portfolios[0].NetGrowthOfWealth.MonthlyGrowthOfWealthReturn[3].Value.Should().Be(expectedGrowthofWealth3);


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
                viewResultPortfolio[0].SixMonth.Should().BeApproximately(
                    expectedSixMonthResult.AsPercent(), 0.00000001m);
		        viewResultPortfolio[0].QuarterToDate.Should().BeApproximately(expectedQuarterToDateResult.AsPercent(), 0.00000001m);
                viewResultPortfolio[0].YearToDate.Should().BeApproximately(expectedYearToDateResult.AsPercent(), 0.00000001m);

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
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult);

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
            });
        }

        [Fact]
        public void ShouldBeAbleToGetCountryForMultiplePortfolios()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber1 = 100;
                var portfolioName1 = "Portfolio 100";

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "None Selected"
                });

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 1,
                    CountryName = "Country 1"
                });

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 2,
                    CountryName = "Country 2"
                });


                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber1,
                    Name = portfolioName1,
                    InceptionDate = new DateTime(2016, 1, 1),
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber1,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });


                var portfolioNumber2 = 101;
                var portfolioName2 = "Portfolio 101";

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber2,
                    Name = portfolioName2,
                    InceptionDate = new DateTime(2016, 1, 1),
                    CountryId= 1
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber2,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var portfolioNumber3 = 102;
                var portfolioName3 = "Portfolio 102";

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber3,
                    Name = portfolioName3,
                    InceptionDate = new DateTime(2016, 1, 1),
                    CountryId = 2
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber3,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });
           

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);
                var countryArray = viewResultModel.Countries.ToArray();

                countryArray.Length.Should().Be(4);

                countryArray[0].Text.Should().Be("Show All");
                countryArray[0].Value.Should().Be("Show All");


                countryArray[1].Text.Should().Be("None Selected");
                countryArray[1].Value.Should().Be("None Selected");
                viewResultModel.Portfolios[0].Country.Should().Be("None Selected");

                countryArray[2].Text.Should().Be("Country 1");
                countryArray[2].Value.Should().Be("Country 1");
                viewResultModel.Portfolios[1].Country.Should().Be("Country 1");

                countryArray[3].Text.Should().Be("Country 2");
                countryArray[3].Value.Should().Be("Country 2");
                viewResultModel.Portfolios[2].Country.Should().Be("Country 2");
            });
        }
    }
}