using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

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


        private const string SelectClause = @"
SELECT
        [ReturnSeriesId]
        ,[Name]
        ,[Disclosure]
    FROM
        [Performance].[ReturnSeries]";

        public ReturnSeriesDto[] GetAll()
        {
            ReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<ReturnSeriesDto>(SelectClause).ToArray();
                },
                SelectClause);

            return result;
        }

        public ReturnSeriesDto[] Get(int[] returnSeriesIds)
        {
            ReturnSeriesDto[] result = null;

            var sql = SelectClause + @"
WHERE
    [ReturnSeriesId] IN @ReturnSeriesIds";

            ConnectionExecuteWithLog(
                connection =>
                {
                    var paramObject = new
                    {
                        ReturnSeriesIds = returnSeriesIds
                    };

                    result = connection.Query<ReturnSeriesDto>(sql, paramObject).ToArray();
                },
                sql);

            return result;
        }


        public int Insert(ReturnSeriesDto dto)
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

        public void Insert(ReturnSeriesDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Performance].[ReturnSeries]
           ([Name]
           ,[Disclosure])
     VALUES
           (@Name
           ,@Disclosure)
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