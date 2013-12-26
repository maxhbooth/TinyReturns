using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Database;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Database
{
    public class ReturnSeriesPersisterTests : DatabaseTestBase
    {
        [Fact]
        public void ShouldReadAndWriteReturnSeries()
        {
            DeleteTestData();
            
            var returnSeries = new ReturnSeries();

            returnSeries.EntityNumber = 100;
            returnSeries.Description = "Test Series 999";
            returnSeries.FeeTypeCode = 'N';

            var tinyReturnsDatabase = new TinyReturnsDatabase(
                MasterFactory.TinyReturnsDatabaseSettings,
                MasterFactory.SystemLog);

            var newId = tinyReturnsDatabase.InsertReturnSeries(returnSeries);

            var savedReturnSeries = tinyReturnsDatabase.GetReturnSeries(newId);

            Assert.NotNull(savedReturnSeries);

            Assert.Equal(savedReturnSeries.ReturnSeriesId, newId);
            Assert.Equal(savedReturnSeries.EntityNumber, returnSeries.EntityNumber);
            Assert.Equal(savedReturnSeries.Description, returnSeries.Description);
            Assert.Equal(savedReturnSeries.FeeTypeCode, returnSeries.FeeTypeCode);

            DeleteTestData();
        }

        private void DeleteTestData()
        {
            const string deleteSql = "DELETE FROM ReturnSeries WHERE Description = 'Test Series 999'";

            ConnectionExecuteWithLog(
                MasterFactory.TinyReturnsDatabaseSettings.ReturnsDatabaseConnectionString,
                connection =>
                {
                    connection.Execute(deleteSql);
                },
                deleteSql);
        }
    }
}