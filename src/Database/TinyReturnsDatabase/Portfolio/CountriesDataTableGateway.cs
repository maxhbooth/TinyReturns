using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio
{
    public class CountriesDataTableGateway : BaseTinyReturnsDataTableGateway, ICountriesDataTableGateway
    {
        public CountriesDataTableGateway(ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog) : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public void Insert(CountryDto[] dtos)
        {
            const string sql = @"
    INSERT INTO [Portfolio].[Countries]
           ([CountryId]
           ,[CountryName]
     VALUES
           (@CountryId
           ,@CountryName
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, dtos);
                },
                sql);
        }

        public CountryDto[] GetAll()
        {
            {
                const string sql = @"
    SELECT
         [CountryId]
        ,[CountryName]
    FROM
         [Portfolio].[PortfolioToBenchmark]";

                CountryDto[] result = null;

                ConnectionExecuteWithLog(
                    connection =>
                    {
                        result = connection.Query<CountryDto>(sql).ToArray();
                    },
                    sql);

                return result;
            }
        }
    }
}