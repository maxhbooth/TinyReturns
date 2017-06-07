﻿using FluentAssertions;
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
        public void ShouldReturnNetWhenNoPortfolioAreFound()
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
                    Selected = "0"
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
                    Selected = "1"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(3);

                for (int i = 0; i < resultModel.Portfolios.Length; i++)
                {
                    testHelper.AssertPortfolioModelIsGross(resultModel.Portfolios[i]);
                    testHelper.AssertModelIsGross(resultModel);
                }
            });
        }
    }
}
