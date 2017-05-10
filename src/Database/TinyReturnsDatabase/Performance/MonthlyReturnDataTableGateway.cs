using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance
{
    public class MonthlyReturnDataTableGateway
        : BaseTinyReturnsDataTableGateway,
        IMonthlyReturnDataTableGateway
    {
        public MonthlyReturnDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        private const string SelecteClause = @"
SELECT
        [ReturnSeriesId]
        ,[Year]
        ,[Month]
        ,[ReturnValue]
    FROM
        [Performance].[MonthlyReturn]";

        public MonthlyReturnDto[] GetAll()
        {
            MonthlyReturnDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<MonthlyReturnDto>(SelecteClause).ToArray();
                },
                SelecteClause);

            return result;
        }

        public MonthlyReturnDto[] Get(int[] returnSeriesIds)
        {
            MonthlyReturnDto[] result = null;

            var sql = SelecteClause + @"
WHERE
    [ReturnSeriesId] IN @ReturnSeriesIds";

            ConnectionExecuteWithLog(
                connection =>
                {
                    var paramObject = new
                    {
                        ReturnSeriesIds = returnSeriesIds
                    };

                    result = connection.Query<MonthlyReturnDto>(sql, paramObject).ToArray();
                },
                sql);

            return result;
        }

        public void Insert(MonthlyReturnDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Performance].[MonthlyReturn]
           ([ReturnSeriesId]
           ,[Year]
           ,[Month]
           ,[ReturnValue])
     VALUES
           (@ReturnSeriesId
           ,@Year
           ,@Month
           ,@ReturnValue)";

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