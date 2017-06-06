using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
                var resultPortfolioModel = testHelper.GetPortfolioModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultPortfolioModel);
                testHelper.AssertSelectItemDefaultsNet(resultModel);
            });

        }

    }
}
