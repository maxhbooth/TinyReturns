using System;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Web.Models;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class PortfolioPerformanceControllerForIndexForPostTests
    {
        [Fact]
        public void ShouldReturnAllRecordsWhenShowAllIsSelected()
        {
            // Arrange
            var testHelper = new PortfolioPerformanceControllerTestHelper();

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
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    SelectedLetter = "0"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertLetterSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(3);
            });
        }

        [Fact]
        public void ShouldReturnZeroRecordsThatWhenFirstLetterDoesNotMatch()
        {
            // Arrange
            var testHelper = new PortfolioPerformanceControllerTestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "ABC",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 101,
                    Name = "EFG",
                    InceptionDate = new DateTime(2010, 1, 1)

                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 102,
                    Name = "HIJ",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    SelectedLetter = "X"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertLetterSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(0);
            });
        }

        [Fact]
        public void ShouldReturnRecordsThatMatchFirstLetter()
        {
            // Arrange
            var testHelper = new PortfolioPerformanceControllerTestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "ABC",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 101,
                    Name = "EFG",
                    InceptionDate = new DateTime(2010, 1, 1)

                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 102,
                    Name = "HIJ",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    SelectedLetter = "E"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertLetterSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(1);
                resultModel.Portfolios[0].Number.Should().Be(101);
            });
        }

        [Fact]
        public void ShouldReturnRecordsThatMatchFirstLetterWithoutCasing()
        {
            // Arrange
            var testHelper = new PortfolioPerformanceControllerTestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 100,
                    Name = "ABC",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 101,
                    Name = "EFG",
                    InceptionDate = new DateTime(2010, 1, 1)

                });

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = 102,
                    Name = "HIJ",
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                // Act
                var requestModel = new PortfolioPerformanceIndexModel()
                {
                    SelectedLetter = "e"
                };

                var actionResult = controller.Index(requestModel);

                actionResult.Should().NotBeNull();

                var resultModel = testHelper.GetModelFromActionResult(actionResult);

                testHelper.AssertLetterSelectItemsArePopulated(resultModel);

                resultModel.Portfolios.Length.Should().Be(1);
                resultModel.Portfolios[0].Number.Should().Be(101);
            });
        }


    }
}