using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

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
           ,[CountryId]
           ,[InceptionDate]
           ,[CloseDate])
     VALUES
           (@Number
           ,@Name
           ,@CountryId
           ,@InceptionDate
           ,@CloseDate)
";

            ConnectionExecuteWithLog(
                connection =>
                {
                    connection.Execute(sql, dto);
                },
                sql,
                dto);
        }

        public void Update(PortfolioDto dto)
        {
            const string sql = @"
UPDATE [Portfolio].[Portfolio]
        SET
           ,[Name] = @Name
           ,[CountryId] = @CountryId
           ,[InceptionDate] = @InceptionDate
           ,[CloseDate] = @CloseDate
        Where
           [Number] = @Number
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
        ,[CountryId]
        ,[Name]
        ,[InceptionDate]
        ,[CloseDate]
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