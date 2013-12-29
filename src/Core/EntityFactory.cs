using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class EntityFactory
    {
        private readonly ReturnSeriesDto[] _returnSeriesDtos;
        private readonly MonthlyReturnDto[] _monthlyReturnDtos;

        public EntityFactory(
            ReturnSeriesDto[] returnSeriesDtos,
            MonthlyReturnDto[] monthlyReturnDtos)
        {
            _monthlyReturnDtos = monthlyReturnDtos;
            _returnSeriesDtos = returnSeriesDtos;
        }

        public Entity CreateEntity(
            EntityDto entityDto)
        {
            var entity = new Entity
            {
                EntityNumber = entityDto.EntityNumber,
                Name = entityDto.Name,
                EntityType = EntityType.FromCode(entityDto.EntityTypeCode)
            };

            entity.ReturnSeries = CreateReturnSeriesForEntity(entityDto);

            return entity;
        }

        private ReturnSeries[] CreateReturnSeriesForEntity(
            EntityDto entityDto)
        {
            var seriesForEntity = _returnSeriesDtos
                .Where(d => d.EntityNumber == entityDto.EntityNumber);

            var returnSeries = seriesForEntity
                .Select(CreateReturnSeries)
                .ToArray();

            return returnSeries;
        }

        private ReturnSeries CreateReturnSeries(
            ReturnSeriesDto r)
        {
            var returnSeries = new ReturnSeries()
            {
                ReturnSeriesId = r.ReturnSeriesId,
                FeeType = FeeType.FromCode(r.FeeTypeCode)
            };

            var returnsForSeries = _monthlyReturnDtos
                .Where(d => d.ReturnSeriesId == r.ReturnSeriesId);

            returnSeries.MonthlyReturns = returnsForSeries
                .Select(CreateMonthlyReturn)
                .ToArray();

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