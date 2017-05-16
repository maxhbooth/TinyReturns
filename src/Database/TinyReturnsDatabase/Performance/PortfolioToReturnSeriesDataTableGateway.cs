using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance
{
    public class PortfolioToReturnSeriesDataTableGateway
        : BaseTinyReturnsDataTableGateway,
        IPortfolioToReturnSeriesDataTableGateway
    {
        public PortfolioToReturnSeriesDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        private const string SelectClause = @"
SELECT
        [PortfolioNumber]
        ,[ReturnSeriesId]
        ,[SeriesTypeCode]
    FROM
        [Performance].[PortfolioToReturnSeries]";

        public PortfolioToReturnSeriesDto[] GetAll()
        {
            PortfolioToReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<PortfolioToReturnSeriesDto>(SelectClause).ToArray();
                },
                SelectClause);

            return result;
        }

        public PortfolioToReturnSeriesDto[] Get(int[] portfolioNumbers)
        {
            PortfolioToReturnSeriesDto[] result = null;

            var sql = SelectClause + @"
WHERE   
    PortfolioNumber IN @PortfolioNumbers";

            ConnectionExecuteWithLog(
                connection =>
                {
                    var paramObject = new
                    {
                        PortfolioNumbers = portfolioNumbers
                    };

                    result = connection.Query<PortfolioToReturnSeriesDto>(sql, paramObject).ToArray();
                },
                sql);

            return result;
        }

        public void Insert(PortfolioToReturnSeriesDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Performance].[PortfolioToReturnSeries]
           ([PortfolioNumber]
           ,[ReturnSeriesId]
           ,[SeriesTypeCode])
     VALUES
           (@PortfolioNumber
           ,@ReturnSeriesId
           ,@SeriesTypeCode)
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