using System.IO;
using System.Linq;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
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
            private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
            private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                _allTablesDeleter = new AllTablesDeleter();

                _returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _monthlyReturnDataTableGateway = new MonthlyReturnDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioToReturnSeriesDataTableGateway = new PortfolioToReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _citiReturnsFileReader = new CitiReturnsFileReader(
                    systemLogForIntegrationTests);
            }

            public ReturnSeriesDto[] GetAllReturnSeriesDtos()
            {
                return _returnSeriesDataTableGateway.GetAll();
            }

            public MonthlyReturnDto[] GetAllMonthlyReturnDtos()
            {
                return _monthlyReturnDataTableGateway.GetAll();
            }

            public PortfolioToReturnSeriesDto[] GetAllPortfolioToReturnSeriesDtos()
            {
                return _portfolioToReturnSeriesDataTableGateway.GetAll();
            }

            public CitiMonthyReturnImporter CreateImporter()
            {

                return new CitiMonthyReturnImporter(
                    _citiReturnsFileReader,
                    _returnSeriesDataTableGateway,
                    _monthlyReturnDataTableGateway,
                    _portfolioToReturnSeriesDataTableGateway);
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

            var filePath = GetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioNetReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();

            Assert.Equal(3, returnSeriesDtos.Length);

            // **

            var returnSeriesPortfolio100 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));

            Assert.NotNull(returnSeriesPortfolio100);
            Assert.True(returnSeriesPortfolio100.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio100.Disclosure);

            // **

            var returnSeriesPortfolio101 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(101));

            Assert.NotNull(returnSeriesPortfolio101);
            Assert.True(returnSeriesPortfolio101.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio101.Disclosure);

            // **

            var returnSeriesPortfolio102 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(102));

            Assert.NotNull(returnSeriesPortfolio102);
            Assert.True(returnSeriesPortfolio102.ReturnSeriesId > 0);
            Assert.Equal(string.Empty, returnSeriesPortfolio102.Disclosure);

            // **

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

            var filePath = GetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioNetReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();

            var returnSeriesPortfolio100 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));

            Assert.NotNull(returnSeriesPortfolio100);

            var allMonthlyReturnDtos = testHelper.GetAllMonthlyReturnDtos();

            var monthlyReturnDtos = allMonthlyReturnDtos
                .Where(d => d.ReturnSeriesId == returnSeriesPortfolio100.ReturnSeriesId)
                .ToArray();

            Assert.Equal(9, monthlyReturnDtos.Length);

            var monthlyReturnDto = monthlyReturnDtos.FirstOrDefault(d => d.Month == 10 && d.Year == 2013);
            Assert.NotNull(monthlyReturnDto);

            Assert.Equal(0.04400550m, monthlyReturnDto.ReturnValue, 5);

            // Teardown
            testHelper.DeleteAllDataInDatabase();
        }

        [Fact(DisplayName = "ImportMonthyPortfolioNetReturnsFile should populate the table Performance.PortfolioToReturnSeries with net record per portfolio.")]
        public void ImportMonthyPortfolioNetReturnsFileShouldPopulatePortfolioToReturnSeries()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DeleteAllDataInDatabase();

            var importer = testHelper.CreateImporter();

            var filePath = GetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioNetReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();
            var portfolioToReturnSeriesDtos = testHelper.GetAllPortfolioToReturnSeriesDtos();

            Assert.Equal(3, returnSeriesDtos.Length);

            // **

            var returnSeriesPortfolio100 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));

            Assert.NotNull(returnSeriesPortfolio100);

            var portfolioToReturnSeriesDto100 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 100
                && d.ReturnSeriesId == returnSeriesPortfolio100.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.NetSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto100);

            // **

            var returnSeriesPortfolio101 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(101));

            Assert.NotNull(returnSeriesPortfolio101);

            var portfolioToReturnSeriesDto101 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 101
                && d.ReturnSeriesId == returnSeriesPortfolio101.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.NetSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto101);

            // **

            var returnSeriesPortfolio102 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(102));

            Assert.NotNull(returnSeriesPortfolio102);

            var portfolioToReturnSeriesDto102 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 102
                && d.ReturnSeriesId == returnSeriesPortfolio102.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.NetSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto102);

            // Teardown
            testHelper.DeleteAllDataInDatabase();
        }

        [Fact(DisplayName = "ImportMonthyPortfolioGrossReturnsFile should populate the table Performance.PortfolioToReturnSeries with gross record per portfolio.")]
        public void ImportMonthyPortfolioGrossReturnsFileShouldPopulatePortfolioToReturnSeries()
        {
            // Arrange
            var testHelper = new TestHelper();

            testHelper.DeleteAllDataInDatabase();

            var importer = testHelper.CreateImporter();

            var filePath = GetReturnsTestFilePath();

            // Act
            importer.ImportMonthyPortfolioGrossReturnsFile(filePath);

            // Assert
            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();
            var portfolioToReturnSeriesDtos = testHelper.GetAllPortfolioToReturnSeriesDtos();

            Assert.Equal(3, returnSeriesDtos.Length);

            // **

            var returnSeriesPortfolio100 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(100));

            Assert.NotNull(returnSeriesPortfolio100);

            var portfolioToReturnSeriesDto100 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 100
                && d.ReturnSeriesId == returnSeriesPortfolio100.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.GrossSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto100);

            // **

            var returnSeriesPortfolio101 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(101));

            Assert.NotNull(returnSeriesPortfolio101);

            var portfolioToReturnSeriesDto101 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 101
                && d.ReturnSeriesId == returnSeriesPortfolio101.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.GrossSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto101);

            // **

            var returnSeriesPortfolio102 = returnSeriesDtos.FirstOrDefault(d =>
                d.Name == CitiMonthyReturnImporter.CreateReturnSeriesName(102));

            Assert.NotNull(returnSeriesPortfolio102);

            var portfolioToReturnSeriesDto102 = portfolioToReturnSeriesDtos.FirstOrDefault(d =>
                d.PortfolioNumber == 102
                && d.ReturnSeriesId == returnSeriesPortfolio102.ReturnSeriesId
                && d.SeriesTypeCode == PortfolioToReturnSeriesDto.GrossSeriesTypeCode);

            Assert.NotNull(portfolioToReturnSeriesDto102);

            // Teardown
            testHelper.DeleteAllDataInDatabase();
        }


        private string GetReturnsTestFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\Core\TestReturnsForEntity100_101_102.csv";

            return targetFile;
        }
    }
}