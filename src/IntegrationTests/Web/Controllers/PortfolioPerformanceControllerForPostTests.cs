using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void ShouldReturnNetByDefault()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = testHelper.GetPortfolioModelFromActionResult(actionResult);

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

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "Portfolio 100",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 101,
                    Name = "Portfolio 101",
                    InceptionDate = new DateTime(2010, 1, 1)

                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 102,
                    Name = "Portfolio 102",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceNetGrossModel()
                {
                    Selected = "Net"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(3);
                //var result = resultModel.Portfolios[0];
                for (int i = 0; i < resultModel.Portfolios.Length; i++)
                {
                    testHelper.AssertPortfolioModelIsNet(resultModel.Portfolios[i]);
                    testHelper.AssertModelIsNet(resultModel);
                }
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

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "Portfolio 100",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 101,
                    Name = "Portfolio 101",
                    InceptionDate = new DateTime(2010, 1, 1)

                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 102,
                    Name = "Portfolio 102",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceNetGrossModel()
                {
                    Selected = "Gross"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(3);
                //var result = resultModel.Portfolios[0];
                for (int i = 0; i < resultModel.Portfolios.Length; i++)
                {
                    testHelper.AssertPortfolioModelIsGross(resultModel.Portfolios[i]);
                    testHelper.AssertModelIsGross(resultModel);
                }
            });
        }
        /*
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
                        var viewResultPortfolioModel = testHelper.GetPortfolioModelFromActionResult(actionResult);

                        viewResultPortfolioModel.Length.Should().Be(1);

                        viewResultPortfolioModel[0].Number.Should().Be(portfolioNumber);
                        viewResultPortfolioModel[0].Name.Should().Be(portfolioName);
                        viewResultPortfolioModel[0].Benchmarks.Should().BeEmpty();

                        viewResultPortfolioModel[0].OneMonth.Should().BeApproximately(0.02m, 0.00001m);
                        viewResultPortfolioModel[0].ThreeMonth.Should().NotHaveValue();
                        viewResultPortfolioModel[0].YearToDate.Should().BeApproximately(0.02m, 0.00001m);
                    });
                }*/


    }
}
