using FluentAssertions;
using System;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Web.Models;
using Xunit;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class PortfolioPerformanceControllerForPostTests
    {
        [Fact]
        public void ShouldReturnNetWhenNoPortfolioAreFound()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = testHelper.GetPortfoliosFromActionResult(actionResult);

                viewResultModel.Length.Should().Be(0);

                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
            });

        }
        [Fact]
        public void ShouldReturnNetValuesWhenNetIsSelected()
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
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    MonthYear = previousMonthYear.Stringify(),
                    SelectedTypeOfReturn = "0"
                };
                int returnId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Month"
                });
                testHelper.InsertMonthlyReturnDto(new MonthlyReturnDto()
                {
                    Month = 1,
                    Year = 2017,
                    ReturnValue = 0.02m,
                    ReturnSeriesId = returnId
                });
                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = 100,
                    ReturnSeriesId = returnId,
                    SeriesTypeCode = 'N'
                });
                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(1);
                resultModel.Portfolios[0].OneMonth.Should().Be(2.00m);
                testHelper.AssertModelIsNet(resultModel);
            });
        }

        [Fact]
        public void ShouldReturnGrossValuesWhenGrossIsSelected()
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
                    InceptionDate = new DateTime(2010, 1, 1)
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
                resultModel.Portfolios[0].OneMonth.Should().Be(2.00m);
                testHelper.AssertModelIsGross(resultModel);


                resultModel.Portfolios[0].NetGrowthOfWealth.Should().BeNull();
                resultModel.Portfolios[0].GrossGrowthOfWealth.MonthlyGrowthOfWealthReturn[0].Value.Should().Be(0.02m);


            });
        }
    }
}
