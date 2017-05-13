using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance
{
    public class BenchmarkToReturnSeriesDataTableGateway
        : BaseTinyReturnsDataTableGateway,
        IBenchmarkToReturnSeriesDataTableGateway
    {
        public BenchmarkToReturnSeriesDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        private const string SelectClause = @"
SELECT
        [BenchmarkNumber]
        ,[ReturnSeriesId]
    FROM
        [Performance].[BenchmarkToReturnSeries]";

        public BenchmarkToReturnSeriesDto[] GetAll()
        {
            BenchmarkToReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<BenchmarkToReturnSeriesDto>(SelectClause).ToArray();
                },
                SelectClause);

            return result;
        }

        public void Insert(BenchmarkToReturnSeriesDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Performance].[BenchmarkToReturnSeries]
           ([BenchmarkNumber]
           ,[ReturnSeriesId])
     VALUES
           (@BenchmarkNumber
           ,@ReturnSeriesId)
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, dtos);
                },
                sql,
                dtos);
        }
    }
}