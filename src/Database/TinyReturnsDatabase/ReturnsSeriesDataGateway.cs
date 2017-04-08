using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase
{
    public class ReturnsSeriesDataGateway : BaseTinyReturnsDataTableGateway, IReturnsSeriesDataGateway
    {
        public ReturnsSeriesDataGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public int InsertReturnSeries(ReturnSeriesDto returnSeries)
        {
            const string sql = @"
INSERT INTO [ReturnSeries]
           ([InvestmentVehicleNumber]
           ,[FeeTypeCode])
     VALUES
           (@InvestmentVehicleNumber
           ,@FeeTypeCode)

SELECT CAST(SCOPE_IDENTITY() as int)
";

            int newId = 0;

            ConnectionExecuteWithLog(
                connection =>
                {
                    newId = connection.Query<int>(sql, returnSeries).Single();
                },
                sql,
                returnSeries);

            return newId;
        }

        public ReturnSeriesDto GetReturnSeries(int returnSeriesId)
        {
            const string sql = @"
SELECT
        [ReturnSeriesId]
        ,[InvestmentVehicleNumber]
        ,[FeeTypeCode]
    FROM
        [ReturnSeries]
    WHERE
        ReturnSeriesId = @ReturnSeriesId";

            ReturnSeriesDto result = null;

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<ReturnSeriesDto>(sql, paramObject).FirstOrDefault();
                },
                sql);

            return result;
        }

        public void DeleteReturnSeries(int returnSeriesId)
        {
            const string deleteSqlTemplate = "DELETE FROM ReturnSeries WHERE ReturnSeriesId = @ReturnSeriesId";

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(deleteSqlTemplate, paramObject);
                },
                deleteSqlTemplate,
                paramObject);
        }

        public ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers)
        {
            const string sqlTemplate = @"
SELECT
        [ReturnSeriesId]
        ,[InvestmentVehicleNumber]
        ,[FeeTypeCode]
    FROM
        [ReturnSeries]
    WHERE
        InvestmentVehicleNumber IN ({0})";

            var commaSepNumbers = entityNumbers
                .Select(n => n.ToString())
                .Aggregate((f, s) => f + "," + s);

            var sql = string.Format(sqlTemplate, commaSepNumbers);

            ReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<ReturnSeriesDto>(sql).ToArray();
                },
                sqlTemplate);

            return result;
        }

        public void DeleteAllReturnSeries()
        {
            const string deleteReturnSeriesSql = "DELETE FROM [ReturnSeries]";

            ConnectionExecuteWithLog(
                connection => connection.Execute(deleteReturnSeriesSql),
                deleteReturnSeriesSql);
        }
    }
}