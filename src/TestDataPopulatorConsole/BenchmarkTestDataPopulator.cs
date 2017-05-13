using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class BenchmarkTestDataPopulator
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

        public BenchmarkTestDataPopulator(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway)
        {
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }
    }
}