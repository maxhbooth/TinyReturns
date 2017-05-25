using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio
{
    public class BenchmarkDataTableGateway
        : BaseTinyReturnsDataTableGateway,
        IBenchmarkDataTableGateway
    {
        public BenchmarkDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public BenchmarkDto[] GetAll()
        {
            const string sql = @"
SELECT
        [Number]
        ,[Name]
    FROM
        [Portfolio].[Benchmark]";

            BenchmarkDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<BenchmarkDto>(sql).ToArray();
                },
                sql);

            return result;
        }

        public void Insert(BenchmarkDto[] dtos)
        {
            const string sql = @"
INSERT INTO [Portfolio].[Benchmark]
           ([Number]
           ,[Name])
     VALUES
           (@Number
           ,@Name)
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