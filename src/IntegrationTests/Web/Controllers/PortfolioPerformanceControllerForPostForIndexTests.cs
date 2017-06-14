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
                    CountryName = "None Selected"
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
                    Selected = "0"
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
                    Selected = "0"
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
                    var portfolioNumber = 100;
                    var portfolioName = "Portfolio 100";

                    var monthYear = new MonthYear(2016, 5);

                    testHelper.InsertPortfolioDto(new PortfolioDto()
                    {
                        Number = portfolioNumber,
                        Name = portfolioName,
                        InceptionDate = new DateTime(2016, 1, 1)
                    });

                    var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                    {
                        Name = "Net Return Series for Portfolio 100"
                    });

                    testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                    {
                        PortfolioNumber = portfolioNumber,
                        ReturnSeriesId = returnSeriesIdNet,
                        SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                    });

                    


                });
        }
    

    [Fact]
        public void ShouldReturnSameValuesForSubmittingUnchangedform()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2016, 1, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                foreach (var monthlyReturn in netMonthlyReturnDtos)
                {
                    Debug.WriteLine("Net Returns:");
                    Debug.WriteLine(monthlyReturn.ReturnValue);
                }

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);




            });

        }

    }
}
