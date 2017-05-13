using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("Console", new DatabaseSettings());

            var portfolioDataTableGateway = MasterFactory.PortfolioDataTableGateway;
            var returnSeriesDataTableGateway = MasterFactory.ReturnSeriesDataTableGateway;
            var monthlyReturnDataTableGateway = MasterFactory.MonthlyReturnDataTableGateway;
            var portfolioToReturnSeriesDataTableGateway = MasterFactory.PortfolioToReturnSeriesDataTableGateway;

            var testDataPopulator = new PortfolioTestDataPopulator(
                portfolioDataTableGateway,
                returnSeriesDataTableGateway,
                monthlyReturnDataTableGateway,
                portfolioToReturnSeriesDataTableGateway);

            testDataPopulator.PopulateTestData();
        }

    }
}
