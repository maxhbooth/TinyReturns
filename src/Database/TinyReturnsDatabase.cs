using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Database
{
    public class TinyReturnsDatabase :
        BaseDatabase,
        IReturnsSeriesDataGateway,
        IMonthlyReturnsDataGateway,
        IInvestmentVehicleDataGateway
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

        public void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns)
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

        public MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId)
        {
            const string sql = @"
SELECT
        [ReturnSeriesId]
        ,[Year]
        ,[Month]
        ,[ReturnValue]
    FROM
        [MonthlyReturn]
    WHERE
        ReturnSeriesId = @ReturnSeriesId";

            MonthlyReturnDto[] result = null;

            var paramObject = new { ReturnSeriesId = returnSeriesId };

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<MonthlyReturnDto>(sql, paramObject).ToArray();
                },
                sql,
                paramObject);

            return result;
        }

        public MonthlyReturnDto[] GetMonthlyReturns(
            int[] returnSeriesIds)
        {
            const string sqlTemplate = @"
SELECT
        [ReturnSeriesId]
        ,[Year]
        ,[Month]
        ,[ReturnValue]
    FROM
        [MonthlyReturn]
    WHERE
        ReturnSeriesId IN ({0})";

            var commaSep = returnSeriesIds
                .Select(s => s.ToString())
                .Aggregate((f, s) => f + ", " + s);

            var sql = string.Format(sqlTemplate, commaSep);

            MonthlyReturnDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<MonthlyReturnDto>(sql).ToArray();
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

        public void DeleteAllMonthlyReturns()
        {
            const string deleteMonthlyReturnsSql = "DELETE FROM [MonthlyReturn]";

            ConnectionExecuteWithLog(
                connection => connection.Execute(deleteMonthlyReturnsSql),
                deleteMonthlyReturnsSql);
        }

        public InvestmentVehicleDto[] GetAllEntities()
        {
            const string sql = @"
SELECT
        [InvestmentVehicleNumber]
        ,[Name]
        ,[InvestmentVehicleTypeCode]
    FROM
        [InvestmentVehicle]";

            InvestmentVehicleDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<InvestmentVehicleDto>(sql).ToArray();
                },
                sql);

            return result;
        }
    }
}