using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("TestDataConsole", new DatabaseSettings());

            var portfolioDataTableGateway = MasterFactory.PortfolioDataTableGateway;
            var benchmarkDataTableGateway = MasterFactory.BenchmarkDataTableGateway;
            var returnSeriesDataTableGateway = MasterFactory.ReturnSeriesDataTableGateway;
            var monthlyReturnDataTableGateway = MasterFactory.MonthlyReturnDataTableGateway;
            var portfolioToReturnSeriesDataTableGateway = MasterFactory.PortfolioToReturnSeriesDataTableGateway;
            var benchmarkToReturnSeriesDataTableGateway = MasterFactory.BenchmarkToReturnSeriesDataTableGateway;

            var portfolioTestDataPopulator = new PortfolioTestDataPopulator(
                portfolioDataTableGateway,
                returnSeriesDataTableGateway,
                monthlyReturnDataTableGateway,
                portfolioToReturnSeriesDataTableGateway);

            var benchmarkTestDataPopulator = new BenchmarkTestDataPopulator(
                benchmarkDataTableGateway,
                returnSeriesDataTableGateway,
                monthlyReturnDataTableGateway,
                benchmarkToReturnSeriesDataTableGateway);
            
            portfolioTestDataPopulator.PopulateTestData();
            benchmarkTestDataPopulator.PopulateTestData();
        }

    }
}
