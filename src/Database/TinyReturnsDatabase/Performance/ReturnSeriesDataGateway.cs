using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance
{
    public class ReturnSeriesDataGateway : BaseTinyReturnsDataTableGateway, IReturnSeriesDataGateway
    {
        public ReturnSeriesDataGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public ReturnSeriesDto[] GetAll()
        {
            const string sql = @"
SELECT
        [ReturnSeriesId]
        ,[Name]
        ,[Disclosure]
    FROM
        [Performance].[ReturnSeries]";

            ReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<ReturnSeriesDto>(sql).ToArray();
                },
                sql);

            return result;
        }

        public void Insert(ReturnSeriesDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Performance].[ReturnSeries]
           ([Name]
           ,[Disclosure])
     VALUES
           (@Name
           , @Disclosure)
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