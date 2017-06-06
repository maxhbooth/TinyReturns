using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    class PortfolioPerformanceControllerForPostTests
    {
        public void ShouldReturnNetByDefault()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var controller = testHelper.CreateController();

                // Act
                var actionResult = controller.Index();

                // Assert
                var viewResultModel = testHelper.GetModelFromActionResult(actionResult);

                viewResultModel.Length.Should().Be(0);


                var resultModel = testHelper.GetModelFromActionResult(actionResult);
                testHelper.AssertSelectItemsArePopulated(resultModel);
            });

        }

    }
}
