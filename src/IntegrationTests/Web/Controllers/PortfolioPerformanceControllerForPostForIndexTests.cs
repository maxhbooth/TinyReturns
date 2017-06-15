using System;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Web.Models;
using FluentAssertions;
using Xunit;
using System.Diagnostics;
using Dimensional.TinyReturns.IntegrationTests.Core;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class PortfolioPerformanceControllerForPostForIndexTests
    {
        [Fact]
        public void ShouldNotReturnInvalidCalculatationsForPastMonth()
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
                    CountryName = "None SelectedTypeOfReturn"
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1),
                    CountryId= 0
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

                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    MonthYear = "2/2016",
                    SelectedTypeOfReturn = "0"
                };

                var actionResult = controller.Index(requestModel);

                // Assert
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult)[0];
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(37);
                viewResultPortfolio.ThreeMonth.Should().NotHaveValue();
                viewResultPortfolio.QuarterToDate.Should().HaveValue();
                viewResultPortfolio.YearToDate.Should().HaveValue();
            });
        }

        [Fact]
        public void ShouldReturnValidCalculatationsForPastMonth()
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

                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    MonthYear = "4/2016",
                    SelectedTypeOfReturn = "0"
                };

                var actionResult = controller.Index(requestModel);

                // Assert
                var viewResultPortfolio = testHelper.GetPortfoliosFromActionResult(actionResult)[0];
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.MonthYears.Count().Should().Be(37);

                var viewResultModelArray = viewResultModel.MonthYears.ToArray();
                viewResultModelArray[0].Value.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[0].Text.Should()
                    .Be(monthYear.Month.ToString() + "/" + monthYear.Year.ToString());
                viewResultModelArray[1].Value.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());
                viewResultModelArray[1].Text.Should()
                    .Be(monthYearMinus1.Month.ToString() + "/" + monthYearMinus1.Year.ToString());

                viewResultPortfolio.ThreeMonth.Should().HaveValue();


            });
        }

        [Fact]
        public void ShouldBeAbleToChangeCountryWithoutMonthlyReturns()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
                {

                    testHelper.InsertCountryDto(new CountryDto()
                    {
                        CountryId = 0,
                        CountryName = "None SelectedTypeOfReturn"
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

                    var portfolioNumber1 = 100;
                    var portfolioName1 = "Portfolio 100";

                    testHelper.InsertPortfolioDto(new PortfolioDto()
                    {
                        Number = portfolioNumber1,
                        Name = portfolioName1,
                        InceptionDate = new DateTime(2016, 1, 1),
                    });

                    var returnSeriesIdNet1 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                    {
                        Name = "Net Return Series for Portfolio 100"
                    });

                    testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                    {
                        PortfolioNumber = portfolioNumber1,
                        ReturnSeriesId = returnSeriesIdNet1,
                        SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                    });

                    var portfolioNumber2 = 101;
                    var portfolioName2 = "Portfolio 101";

                    testHelper.InsertPortfolioDto(new PortfolioDto()
                    {
                        Number = portfolioNumber2,
                        Name = portfolioName2,
                        InceptionDate = new DateTime(2016, 1, 1),
                        CountryId = 1
                    });

                    var returnSeriesIdNet2 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                    {
                        Name = "Net Return Series for Portfolio 101"
                    });

                    testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                    {
                        PortfolioNumber = portfolioNumber2,
                        ReturnSeriesId = returnSeriesIdNet2,
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

                    var returnSeriesIdNet3 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                    {
                        Name = "Net Return Series for Portfolio 102"
                    });

                    testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                    {
                        PortfolioNumber = portfolioNumber3,
                        ReturnSeriesId = returnSeriesIdNet3,
                        SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                    });

                    var controller = testHelper.CreateController();

                    // Act
                    var actionResult = controller.Index(new PortfolioPerformanceIndexModel()
                    {
                        SelectedCountry = "None SelectedTypeOfReturn",
                        MonthYear = "4/2016",
                        SelectedTypeOfReturn = "0"

                    });

                    var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                    viewResultModel.Countries.Count().Should().Be(4);
                    viewResultModel.Portfolios.Length.Should().Be(1);
                    viewResultModel.Portfolios[0].Country.Should().Be("None SelectedTypeOfReturn");
                    viewResultModel.Portfolios[0].Number.Should().Be(100);

                });
        }
    

    [Fact]
        public void ShouldReturnSameValuesForSubmittingUnchangedform()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var monthYear = new MonthYear(2016, 8);

                testHelper.InsertCountryDto(new CountryDto()
                {
                    CountryId = 0,
                    CountryName = "None SelectedTypeOfReturn"
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

                var portfolioNumber1 = 100;
                var portfolioName1 = "Portfolio 100";

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber1,
                    Name = portfolioName1,
                    InceptionDate = new DateTime(2016, 1, 1),
                });

                var returnSeriesIdNet1 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber1,
                    ReturnSeriesId = returnSeriesIdNet1,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var portfolioNumber2 = 101;
                var portfolioName2 = "Portfolio 101";

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber2,
                    Name = portfolioName2,
                    InceptionDate = new DateTime(2016, 1, 1),
                    CountryId = 1
                });

                var returnSeriesIdNet2 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 101"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber2,
                    ReturnSeriesId = returnSeriesIdNet2,
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

                var returnSeriesIdNet3 = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 102"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber3,
                    ReturnSeriesId = returnSeriesIdNet3,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });


                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var netMonthlyReturnDtos1 = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet1,
                    monthYearRange);

                var netMonthlyReturnDtos2 = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet2,
                    monthYearRange);

                var netMonthlyReturnDtos3 = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet3,
                    monthYearRange);

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos1);
                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos2);
                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos3);

                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index(new PortfolioPerformanceIndexModel()
                {
                    SelectedCountry = "Country 2",
                    MonthYear = "4/2016",
                    SelectedTypeOfReturn = "0"

                });

                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.Countries.Count().Should().Be(4);
                viewResultModel.Portfolios.Length.Should().Be(1);
                viewResultModel.Portfolios[0].Country.Should().Be("Country 2");
                viewResultModel.Portfolios[0].Number.Should().Be(102);

            });
        }
    }
}
