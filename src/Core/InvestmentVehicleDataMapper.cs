using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class InvestmentVehicleDataMapper
    {
        private readonly ReturnSeriesDto[] _returnSeriesDtos;
        private readonly MonthlyReturnDto[] _monthlyReturnDtos;

        public InvestmentVehicleDataMapper(
            ReturnSeriesDto[] returnSeriesDtos,
            MonthlyReturnDto[] monthlyReturnDtos)
        {
            _monthlyReturnDtos = monthlyReturnDtos;
            _returnSeriesDtos = returnSeriesDtos;
        }

        public InvestmentVehicle CreateEntity(
            InvestmentVehicleDto investmentVehicleDto)
        {
            var entity = new InvestmentVehicle
            {
                Number = investmentVehicleDto.InvestmentVehicleNumber,
                Name = investmentVehicleDto.Name,
                InvestmentVehicleType = InvestmentVehicleType.FromCode(investmentVehicleDto.InvestmentVehicleTypeCode)
            };

            entity.AddReturnSeries(CreateReturnSeriesForEntity(investmentVehicleDto));

            return entity;
        }

        private MonthlyReturnSeries[] CreateReturnSeriesForEntity(
            InvestmentVehicleDto investmentVehicleDto)
        {
            var seriesForEntity = _returnSeriesDtos
                .Where(d => d.InvestmentVehicleNumber == investmentVehicleDto.InvestmentVehicleNumber);

            var returnSeries = seriesForEntity
                .Select(CreateReturnSeries)
                .ToArray();

            return returnSeries;
        }

        private MonthlyReturnSeries CreateReturnSeries(
            ReturnSeriesDto r)
        {
            var returnSeries = new MonthlyReturnSeries()
            {
                ReturnSeriesId = r.ReturnSeriesId,
                FeeType = FeeType.FromCode(r.FeeTypeCode)
            };

            var returnsForSeries = _monthlyReturnDtos
                .Where(d => d.ReturnSeriesId == r.ReturnSeriesId);

            returnSeries.AddReturns(
                returnsForSeries.Select(CreateMonthlyReturn));

            return returnSeries;
        }

        private MonthlyReturn CreateMonthlyReturn(
            MonthlyReturnDto d)
        {
            return new MonthlyReturn()
            {
                ReturnSeriesId = d.ReturnSeriesId,
                MonthYear = MonthYear.CreateMonthYearFrom(d),
                ReturnValue = d.ReturnValue
            };
        }

    }
}