using System;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Web.Models;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class PortfolioPerformanceChartDataTests
    {
        [Fact]
        public void ShouldReturnNullChartDataWithoutMonthlyReturnsEntered()
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

                var ChartData = testHelper.CreateChartData();

                ChartData.GetNetChartData()[0].GrowthOfWealth.Should().BeNull();
                ChartData.GetGrossChartData()[0].GrowthOfWealth.Should().BeNull();

                viewResultPortfolio[0].Number.Should().Be(100);
                viewResultPortfolio[0].Name.Should().Be("Portfolio 100");
                viewResultPortfolio[0].Benchmarks.Should().BeEmpty();
            });
        }

        [Fact]
        public void ShouldReturnNetChartDataForPastMonth()
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

                var ChartData = testHelper.CreateChartData();

                var expectedGrowthofWealth1 = (1 - 0.01m) * (1 + 0.01m) - 1;
                var expectedGrowthofWealth2 = (1 - 0.01m) * (1 + 0.01m) * (1 + 0.04m) - 1;
                var expectedGrowthofWealth3 = (1 - 0.01m) * (1 + 0.01m) * (1 + 0.04m) * (1 - 0.02m) - 1;

                ChartData.GetNetChartData()[0].GrowthOfWealth.MonthlyGrowthOfWealthReturn[0].Value.Should().Be(-0.01m);
                ChartData.GetNetChartData()[0].GrowthOfWealth.MonthlyGrowthOfWealthReturn[1].Value.Should()
                    .Be(expectedGrowthofWealth1);
                ChartData.GetNetChartData()[0].GrowthOfWealth.MonthlyGrowthOfWealthReturn[2].Value.Should()
                    .Be(expectedGrowthofWealth2);
                ChartData.GetNetChartData()[0].GrowthOfWealth.MonthlyGrowthOfWealthReturn[3].Value.Should()
                    .Be(expectedGrowthofWealth3);

            });
        }

        [Fact]
        public void ShouldReturnGrossChartDataWhenGrossIsSelected()
        {
            //Arrange
            var testHelper = new TestHelper();
            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                var currentMonthYear = new MonthYear(testHelper.CurrentDate);
                var previousMonthYear = currentMonthYear.AddMonths(-1);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "Portfolio 100",
                    InceptionDate = new DateTime(2010, 1, 1),

                });

                // Act
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    MonthYear = previousMonthYear.Stringify(),
                    SelectedTypeOfReturn = "1"
                };

                int returnId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Month"
                });

                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    Month = previousMonthYear.Month,
                    Year = previousMonthYear.Year,
                    ReturnValue = 0.02m,
                    ReturnSeriesId = returnId
                });
                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = 100,
                    ReturnSeriesId = returnId,
                    SeriesTypeCode = 'G'
                });
                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(1);
                resultModel.Portfolios[0].OneMonth.Value.Should().Be(2.00m);
                testHelper.AssertModelIsGross(resultModel);

                var ChartData = testHelper.CreateChartData();
                var check1 = ChartData.GetNetChartData();
                var check2 = ChartData.GetGrossChartData();

                ChartData.GetGrossChartData()[0].GrowthOfWealth.MonthlyGrowthOfWealthReturn[0].Value.Should().Be(0.02m);


            });
        }
    }
}