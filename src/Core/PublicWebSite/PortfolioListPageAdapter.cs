using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PublicWebSite
{
    public class PortfolioListPageAdapter
    {
        private readonly IInvestmentVehicleReturnsRepository _investmentVehicleReturnsRepository;

        public PortfolioListPageAdapter(
            IInvestmentVehicleReturnsRepository investmentVehicleReturnsRepository)
        {
            _investmentVehicleReturnsRepository = investmentVehicleReturnsRepository;
        }

        public PortfolioListRecord[] GetPortfolioPageRecords(
            MonthYear monthYear)
        {
            var portfolios = _investmentVehicleReturnsRepository.GetPortfolios();

            return portfolios
                .Select(portfolio => CreatePortfolioRecord(monthYear, portfolio))
                .ToArray();
        }

        private PortfolioListRecord CreatePortfolioRecord(
            MonthYear monthYear,
            InvestmentVehicle portfolio)
        {
            var oneMonth = CalculateNetOfFeesReturn(portfolio,
                CalculateReturnRequestFactory.OneMonth(monthYear));

            var threeMonth = CalculateNetOfFeesReturn(portfolio,
                CalculateReturnRequestFactory.ThreeMonth(monthYear));

            var yearToDate = CalculateNetOfFeesReturn(portfolio,
                CalculateReturnRequestFactory.YearToDate(monthYear));

            var record = new PortfolioListRecord()
            {
                PortfolioNumber = portfolio.Number,
                PortfolioName = portfolio.Name,
                OneMonth = oneMonth,
                ThreeMonth = threeMonth,
                YearToDate = yearToDate
            };

            return record;
        }

        private SerializableReturnResult CalculateNetOfFeesReturn(
            InvestmentVehicle portfolio,
            CalculateReturnRequest request)
        {
            var result = portfolio.CalculateReturn(request, FeeType.NetOfFees);

            return new SerializableReturnResult(result);
        }
    }
}