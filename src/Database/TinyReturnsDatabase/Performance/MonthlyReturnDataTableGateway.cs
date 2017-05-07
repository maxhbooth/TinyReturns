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

        public MonthlyReturnDto[] GetAll()
        {
            const string sql = @"
SELECT
        [ReturnSeriesId]
        ,[Year]
        ,[Month]
        ,[ReturnValue]
    FROM
        [Performance].[MonthlyReturn]";

            MonthlyReturnDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<MonthlyReturnDto>(sql).ToArray();
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