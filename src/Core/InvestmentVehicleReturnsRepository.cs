using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core
{
    public class InvestmentVehicleReturnsRepository
    {
        private readonly IInvestmentVehicleDataRepository _investmentVehicleDataRepository;
        private readonly IReturnsSeriesDataRepository _returnsSeriesDataRepository;
        private readonly IMonthlyReturnsDataRepository _monthlyReturnsDataRepository;

        public InvestmentVehicleReturnsRepository(
            IInvestmentVehicleDataRepository investmentVehicleDataRepository,
            IReturnsSeriesDataRepository returnsSeriesDataRepository,
            IMonthlyReturnsDataRepository monthlyReturnsDataRepository)
        {
            _monthlyReturnsDataRepository = monthlyReturnsDataRepository;
            _returnsSeriesDataRepository = returnsSeriesDataRepository;
            _investmentVehicleDataRepository = investmentVehicleDataRepository;
        }

        public InvestmentVehicle[] GetEntitiesWithReturnSeries()
        {
            var entityDtos = _investmentVehicleDataRepository.GetAllEntities();

            var allReturnSeriesDtos = GetReturnSeriesDtos(entityDtos);
            var allMonthlyReturnDtos = GetMonthlyReturns(allReturnSeriesDtos);

            var dtosSource = new InvestmentVehicleFactory(allReturnSeriesDtos, allMonthlyReturnDtos);

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

            var returnSeriesDtos = _returnsSeriesDataRepository
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

            var monthlyReturnDtos = _monthlyReturnsDataRepository
                .GetMonthlyReturns(distinctReturnSeriesIds);

            return monthlyReturnDtos;
        }
    }
}