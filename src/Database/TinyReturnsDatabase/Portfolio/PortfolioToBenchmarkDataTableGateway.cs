using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio
{
    public class PortfolioToBenchmarkDataTableGateway
        : BaseTinyReturnsDataTableGateway,
        IPortfolioToBenchmarkDataTableGateway
    {
        public PortfolioToBenchmarkDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public PortfolioToBenchmarkDto[] GetAll()
        {
            const string sql = @"
SELECT
        [PortfolioNumber]
        ,[BenchmarkNumber]
        ,[SortOrder]
    FROM
        [Portfolio].[PortfolioToBenchmark]";

            PortfolioToBenchmarkDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<PortfolioToBenchmarkDto>(sql).ToArray();
                },
                sql);

            return result;
        }

        public void Insert(PortfolioToBenchmarkDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Portfolio].[PortfolioToBenchmark]
           ([PortfolioNumber]
           ,[BenchmarkNumber]
           ,[SortOrder])
     VALUES
           (@PortfolioNumber
           ,@BenchmarkNumber
           ,@SortOrder)
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, dtos);
                },
                sql);
        }
    }
}