using System;
using System.IO;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.CitiFileImport
{
    public class CitiMonthyReturnImporterTests
    {
        [Fact]
        public void ShouldWork()
        {
            var allTablesDeleter = new AllTablesDeleter();

            var databaseSettings = new DatabaseSettings();
            var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

            allTablesDeleter.DeleteAllDataFromTables(
                databaseSettings.TinyReturnsDatabaseConnectionString,
                new AllTablesDeleter.TableInfoDto[0]);

            var portfolioDataTableGateway = new PortfolioDataTableGateway(
                databaseSettings,
                systemLogForIntegrationTests);

            portfolioDataTableGateway.Insert(new PortfolioDto()
            {
                Number = 100,
                Name = "Portfolio 100",
                InceptionDate = new DateTime(2000, 1, 1)
            });

            var netReturnsTestFilePath = GetNetReturnsTestFilePath();

            var returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                databaseSettings,
                systemLogForIntegrationTests);

            var citiMonthyReturnImporter = new CitiMonthyReturnImporter(
                returnSeriesDataTableGateway);

            citiMonthyReturnImporter.ImportMonthyPortfolioNetReturnsFile(
                netReturnsTestFilePath);

            var returnSeriesDtos = returnSeriesDataTableGateway.GetAll();

            Assert.Equal(1, returnSeriesDtos.Length);
            Assert.False(returnSeriesDtos[0].ReturnSeriesId <= 0);
            Assert.Equal("Returns for Portfolio 100", returnSeriesDtos[0].Name);

            allTablesDeleter.DeleteAllDataFromTables(
                databaseSettings.TinyReturnsDatabaseConnectionString,
                new AllTablesDeleter.TableInfoDto[0]);
        }

        private string GetNetReturnsTestFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\Core\TestNetReturnsForEntity100_101_102.csv";

            return targetFile;
        }
    }
}