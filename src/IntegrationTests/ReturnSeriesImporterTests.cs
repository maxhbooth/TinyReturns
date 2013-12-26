using System.IO;
using Dapper;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class ReturnSeriesImporterTests : DatabaseTestBase
    {
        [Fact]
        public void ShouldImportDataWhenGivenValidFile()
        {
            var returnSeries = new ReturnSeries();

            returnSeries.EntityNumber = 100;
            returnSeries.Description = "Test Series 999";
            returnSeries.FeeTypeCode = 'N';

            var returnsSeriesRepository = MasterFactory.GetReturnsSeriesRepository();

            var newReturnSeriesId = returnsSeriesRepository.InsertReturnSeries(returnSeries);
            
            var importer = new ReturnSeriesImporter();

            var filePath = GetTestDataPath();

//            importer.ImportReturnSeries(filePath);
        }

        private string GetTestDataPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\IntegrationTests\NetReturns.csv";

            return targetFile;
        }

        private void DeleteReturnSeriesData()
        {
            const string deleteSql = "DELETE FROM ReturnSeries WHERE Description = 'Test Series 999'";

            ConnectionExecuteWithLog(
                MasterFactory.GetTinyReturnsDatabaseSettings().ReturnsDatabaseConnectionString,
                connection =>
                {
                    connection.Execute(deleteSql);
                },
                deleteSql);
        }

    }

    public class ReturnSeriesImporter
    {
    }
}