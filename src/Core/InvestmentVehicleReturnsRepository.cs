using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core
{
    public class InvestmentVehicleReturnsRepository
    {
        private readonly IInvestmentVehicleDataGateway _investmentVehicleDataGateway;
        private readonly IReturnsSeriesDataGateway _returnsSeriesDataGateway;
        private readonly IMonthlyReturnsDataGateway _monthlyReturnsDataGateway;

        public InvestmentVehicleReturnsRepository(
            IInvestmentVehicleDataGateway investmentVehicleDataGateway,
            IReturnsSeriesDataGateway returnsSeriesDataGateway,
            IMonthlyReturnsDataGateway monthlyReturnsDataGateway)
        {
            _monthlyReturnsDataGateway = monthlyReturnsDataGateway;
            _returnsSeriesDataGateway = returnsSeriesDataGateway;
            _investmentVehicleDataGateway = investmentVehicleDataGateway;
        }

        public virtual InvestmentVehicle[] GetAllInvestmentVehicles()
        {
            var entityDtos = _investmentVehicleDataGateway.GetAllEntities();

            var allReturnSeriesDtos = GetReturnSeriesDtos(entityDtos);
            var allMonthlyReturnDtos = GetMonthlyReturns(allReturnSeriesDtos);

            var dtosSource = new InvestmentVehicleDataAdapter(allReturnSeriesDtos, allMonthlyReturnDtos);

            return entityDtos
                .Select(dtosSource.CreateEntity)
                .ToArray();
        }

        private ReturnSeriesDto[] GetReturnSeriesDtos(
            InvestmentVehicleDto[] investmentVehicleDtos)
        {
            var distinctEntityNumbers = investmentVehicleDtos
                .Select(d => d.InvestmentVehicleNumber)
                .Distinct()
                .ToArray();

            var returnSeriesDtos = _returnsSeriesDataGateway
                .GetReturnSeries(distinctEntityNumbers);

            return returnSeriesDtos;
        }

        private MonthlyReturnDto[] GetMonthlyReturns(
            ReturnSeriesDto[] returnSeries)
        {
            var distinctReturnSeriesIds = returnSeries
                .Select(d => d.ReturnSeriesId)
                .Distinct()
                .ToArray();

            var monthlyReturnDtos = _monthlyReturnsDataGateway
                .GetMonthlyReturns(distinctReturnSeriesIds);

            return monthlyReturnDtos;
        }
    }

    public static class InvestmentVehicleReturnsRepositoryExtensions
    {
        public static InvestmentVehicle[] GetPortfolios(
            this InvestmentVehicleReturnsRepository repository)
        {
            var allVehicles = repository.GetAllInvestmentVehicles();

            return allVehicles.Where(v => v.InvestmentVehicleType == InvestmentVehicleType.Portfolio).ToArray();
        }
    }
}