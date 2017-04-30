using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio
{
    public class PortfolioDataTableGateway :
        BaseTinyReturnsDataTableGateway,
        IPortfolioDataTableGateway
    {
        public PortfolioDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog)
            : base(tinyReturnsDatabaseSettings, systemLog)
        {
        }

        public void Insert(PortfolioDto dto)
        {
            const string sql = @"
INSERT INTO [Portfolio].[Portfolio]
           ([Number]
           ,[Name]
           ,[InceptionDate]
           ,[CloseDate]
           ,[NetReturnSeriesId]
           ,[GrossReturnSeriesId])
     VALUES
           (@Number
           ,@Name
           ,@InceptionDate
           ,@CloseDate
           ,@NetReturnSeriesId
           ,@GrossReturnSeriesId)
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, dto);
                },
                sql,
                dto);
        }

        public PortfolioDto[] GetAll()
        {
            const string sql = @"
SELECT
        [Number]
        ,[Name]
        ,[InceptionDate]
        ,[CloseDate]
        ,[NetReturnSeriesId]
        ,[GrossReturnSeriesId]
    FROM
        [Portfolio].[Portfolio]";

            PortfolioDto[] result = null;

            ConnectionExecuteWithLog(
                connection =>
                {
                    result = connection.Query<PortfolioDto>(sql).ToArray();
                },
                sql);

            return result;
        }
    }
}