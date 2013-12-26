using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.Database
{
    public class TinyReturnsDatabase : BaseDatabase, IReturnsSeriesRepository
    {
        private readonly ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;

        public TinyReturnsDatabase(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog) : base(systemLog)
        {
            _tinyReturnsDatabaseSettings = tinyReturnsDatabaseSettings;
        }

        protected override string DefaultConnectionString
        {
            get { return _tinyReturnsDatabaseSettings.ReturnsDatabaseConnectionString; }
        }

        public int InsertReturnSeries(ReturnSeries returnSeries)
        {
            const string sql = @"
INSERT INTO [ReturnSeries]
           ([EntityNumber]
           ,[Description]
           ,[FeeTypeCode])
     VALUES
           (@EntityNumber
           ,@Description
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

        public ReturnSeries GetReturnSeries(int returnSeriesId)
        {
            const string sql = @"
SELECT [ReturnSeriesId]
      ,[EntityNumber]
      ,[Description]
      ,[FeeTypeCode]
  FROM [ReturnSeries]
    WHERE ReturnSeriesId = @ReturnSeriesId";

            ReturnSeries result = null;

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<ReturnSeries>(sql, paramObject).FirstOrDefault();
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

        public void InsertMonthlyReturns(MonthlyReturn[] monthlyReturns)
        {
            const string sql = @"
INSERT INTO [MonthlyReturn]
           ([ReturnSeriesId]
           ,[Year]
           ,[Month]
           ,[ReturnValue])
     VALUES
           (@ReturnSeriesId
           ,@Year
           ,@Month
           ,@ReturnValue)
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, monthlyReturns);
                },
                sql,
                monthlyReturns);
        }

        public MonthlyReturn[] GetMonthlyReturns(int returnSeriesId)
        {
            const string sql = @"
SELECT [ReturnSeriesId]
      ,[Year]
      ,[Month]
      ,[ReturnValue]
  FROM [MonthlyReturn]
    WHERE ReturnSeriesId = @ReturnSeriesId";

            MonthlyReturn[] result = null;

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<MonthlyReturn>(sql, paramObject).ToArray();
                },
                sql);

            return result;
        }

        public void DeleteMonthlyReturns(int returnSeriesId)
        {
            const string deleteSqlTemplate = "DELETE FROM MonthlyReturn WHERE ReturnSeriesId = @ReturnSeriesId";

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(deleteSqlTemplate, paramObject);
                },
                deleteSqlTemplate,
                paramObject);
        }
    }
}