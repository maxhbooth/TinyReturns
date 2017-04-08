using System.Linq;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Core
{
    public interface IInvestmentVehicleReturnsRepository
    {
        InvestmentVehicle[] GetAllInvestmentVehicles();
    }

    public class InvestmentVehicleReturnsRepository : IInvestmentVehicleReturnsRepository
    {
        private readonly IInvestmentVehicleDataTableGateway _investmentVehicleDataTableGateway;
        private readonly IReturnsSeriesDataTableGateway _returnsSeriesDataTableGateway;
        private readonly IMonthlyReturnsDataTableGateway _monthlyReturnsDataTableGateway;

        public InvestmentVehicleReturnsRepository(
            IInvestmentVehicleDataTableGateway investmentVehicleDataTableGateway,
            IReturnsSeriesDataTableGateway returnsSeriesDataTableGateway,
            IMonthlyReturnsDataTableGateway monthlyReturnsDataTableGateway)
        {
            _monthlyReturnsDataTableGateway = monthlyReturnsDataTableGateway;
            _returnsSeriesDataTableGateway = returnsSeriesDataTableGateway;
            _investmentVehicleDataTableGateway = investmentVehicleDataTableGateway;
        }

        public InvestmentVehicle[] GetAllInvestmentVehicles()
        {
            var entityDtos = _investmentVehicleDataTableGateway.GetAllEntities();

            var allReturnSeriesDtos = GetReturnSeriesDtos(entityDtos);
            var allMonthlyReturnDtos = GetMonthlyReturns(allReturnSeriesDtos);

            var dtosSource = new InvestmentVehicleDataMapper(allReturnSeriesDtos, allMonthlyReturnDtos);

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

            var returnSeriesDtos = _returnsSeriesDataTableGateway
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

            var monthlyReturnDtos = _monthlyReturnsDataTableGateway
                .GetMonthlyReturns(distinctReturnSeriesIds);

            return monthlyReturnDtos;
        }
    }

    public static class InvestmentVehicleReturnsRepositoryExtensions
    {
        public static InvestmentVehicle[] GetPortfolios(
            this IInvestmentVehicleReturnsRepository repository)
        {
            var allVehicles = repository.GetAllInvestmentVehicles();

            return allVehicles.Where(v => v.InvestmentVehicleType == InvestmentVehicleType.Portfolio).ToArray();
        }
    }
}