using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase
{
    public class InvestmentVehicleDataTableGateway : BaseTinyReturnsDataTableGateway, IInvestmentVehicleDataTableGateway
    {
        public InvestmentVehicleDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
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