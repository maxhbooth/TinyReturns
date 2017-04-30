using System;
using System.IO;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.CitiFileImport
{
    public class CitiMonthyReturnImporterTests
    {
        public class TestHelper
        {
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
            private readonly AllTablesDeleter _allTablesDeleter;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                _allTablesDeleter = new AllTablesDeleter();

                _returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioDataTableGateway = new PortfolioDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);
            }

            public void InsertPortfolioDto(PortfolioDto dto)
            {
                _portfolioDataTableGateway.Insert(dto);
            }

            public ReturnSeriesDto[] GetAllReturnSeriesDtos()
            {
                return _returnSeriesDataTableGateway.GetAll();
            }

            public CitiMonthyReturnImporter CreateImporter()
            {

                return new CitiMonthyReturnImporter(
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

        [Fact]
        public void ShouldWork()
        {
            var testHelper = new TestHelper();

            testHelper.DeleteAllDataInDatabase();

            testHelper.InsertPortfolioDto(new PortfolioDto()
            {
                Number = 100,
                Name = "Portfolio 100",
                InceptionDate = new DateTime(2000, 1, 1)
            });

            var importer = testHelper.CreateImporter();

            var netReturnsTestFilePath = GetNetReturnsTestFilePath();

            importer.ImportMonthyPortfolioNetReturnsFile(
                netReturnsTestFilePath);

            var returnSeriesDtos = testHelper.GetAllReturnSeriesDtos();

            Assert.Equal(1, returnSeriesDtos.Length);
            Assert.False(returnSeriesDtos[0].ReturnSeriesId <= 0);
            Assert.Equal("Returns for Portfolio 100", returnSeriesDtos[0].Name);

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