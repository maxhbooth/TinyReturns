using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

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

        public PortfolioToReturnSeriesDto[] GetAll()
        {
            const string sql = @"
SELECT
        [PortfolioNumber]
        ,[ReturnSeriesId]
        ,[SeriesTypeCode]
    FROM
        [Performance].[PortfolioToReturnSeries]";

            PortfolioToReturnSeriesDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<PortfolioToReturnSeriesDto>(sql).ToArray();
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