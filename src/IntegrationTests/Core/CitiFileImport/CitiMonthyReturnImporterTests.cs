using System.IO;
using System.Linq;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.FileIo;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.CitiFileImport
{
    public class CitiMonthyReturnImporterTests
    {
        public class TestHelper
        {
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly AllTablesDeleter _allTablesDeleter;
            private readonly CitiReturnsFileReader _citiReturnsFileReader;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                _allTablesDeleter = new AllTablesDeleter();

                _returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _citiReturnsFileReader = new CitiReturnsFileReader(
                    systemLogForIntegrationTests);
            }

            public ReturnSeriesDto[] GetAllReturnSeriesDtos()
            {
                return _returnSeriesDataTableGateway.GetAll();
            }

            public CitiMonthyReturnImporter CreateImporter()
            {

                return new CitiMonthyReturnImporter(
                    _citiReturnsFileReader,
                    _returnSeriesDataTableGateway);
            }

            public void DeleteAllDataInDatabase()
            {
                var databaseSettings = new DatabaseSettings();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);
            }
        }

        [Fact(DisplayName = "ImportMonthyPortfolioNetReturnsFile should populate the table Performance.ReturnSeries with series from file.")]
        public void ImportMonthyPortfolioNetReturnsFileShouldPopulateReturnSeries()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DeleteAllDataInDatabase();

            var importer = testHelper.CreateImporter();

            var filePath = GetNetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioNetReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();

            Assert.Equal(3, returnSeriesDtos.Length);

            var returnSeriesPortfolio100 = returnSeriesDtos.First(d => d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));
            Assert.NotNull(returnSeriesPortfolio100);
            Assert.True(returnSeriesPortfolio100.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio100.Disclosure);

            var returnSeriesPortfolio101 = returnSeriesDtos.First(d => d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(101));
            Assert.NotNull(returnSeriesPortfolio101);
            Assert.True(returnSeriesPortfolio101.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio101.Disclosure);

            var returnSeriesPortfolio102 = returnSeriesDtos.First(d => d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(102));
            Assert.NotNull(returnSeriesPortfolio102);
            Assert.True(returnSeriesPortfolio102.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio102.Disclosure);

            // Teardown
            testHelper.DeleteAllDataInDatabase();
        }

        [Fact(DisplayName = "ImportMonthyPortfolioNetReturnsFile should populate the table Performance.MonthlyReturn with monthly returns from file.")]
        public void ImportMonthyPortfolioNetReturnsFileShouldPopulateMonthlyReturns()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DeleteAllDataInDatabase();

            var importer = testHelper.CreateImporter();

            var filePath = GetNetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioNetReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();

            Assert.Equal(3, returnSeriesDtos.Length);

            var returnSeriesPortfolio100 = returnSeriesDtos.First(d => d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));
            Assert.NotNull(returnSeriesPortfolio100);

            // Teardown
            testHelper.DeleteAllDataInDatabase();
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