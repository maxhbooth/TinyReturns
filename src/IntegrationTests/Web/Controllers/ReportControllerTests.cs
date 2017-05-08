using Dimensional.TinyReturns.Web.Controllers;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class ReportControllerTests
    {
        public class TestHelper
        {
            public ReportController CreateController()
            {
                return new ReportController();
            }
        }

        [Fact]
        public void ShouldWork()
        {
            var testHelper = new TestHelper();

            var controller = testHelper.CreateController();

            var actionResult = controller.Index();

            Assert.Null(actionResult);
        }
    }
}