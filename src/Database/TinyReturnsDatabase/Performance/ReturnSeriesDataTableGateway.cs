using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance
{
    public class ReturnSeriesDataTableGateway : BaseTinyReturnsDataTableGateway, IReturnSeriesDataTableGateway
    {
        public ReturnSeriesDataTableGateway(
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

        public int Inert(ReturnSeriesDto dto)
        {
            const string sql = @"
INSERT INTO [Performance].[ReturnSeries]
           ([Name]
           ,[Disclosure])
     VALUES
           (@Name
           ,@Disclosure)

SELECT CAST(SCOPE_IDENTITY() as int)
";

            int newId = 0;

            ConnectionExecuteWithLog(
                connection =>
                {
                    newId = connection.Query<int>(sql, dto).Single();
                },
                sql,
                dto);

            return newId;
        }
    }
}