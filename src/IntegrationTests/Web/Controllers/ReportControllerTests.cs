using System;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Web.Controllers;
using FluentAssertions;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Web.Controllers
{
    public class ReportControllerTests
    {
        public class TestHelper
        {
            private readonly AllTablesDeleter _allTablesDeleter;

            public TestHelper()
            {
                _allTablesDeleter = new AllTablesDeleter();
            }

            public ReportController CreateController()
            {
                return new ReportController();
            }

            public void DatabaseDataDeleter(
                Action act)
            {
                var databaseSettings = new DatabaseSettings();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);

                act();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);
            }
        }

        [Fact]
        public void ShouldReturnNoRecrodsWhenNoPortfolioAreFound()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
                {
                    var controller = testHelper.CreateController();

                    // Act
                    var actionResult = controller.Index();

                    // Assert
                    actionResult.Should().BeAssignableTo<ViewResult>();
                    var viewResult = (ViewResult) actionResult;
                    viewResult.Model.Should().BeAssignableTo<PublicWebReportFacade.PortfolioModel[]>();
                    var viewResultModel = (PublicWebReportFacade.PortfolioModel[]) viewResult.Model;

                    viewResultModel.Length.Should().Be(0);
                });
        }
    }
}