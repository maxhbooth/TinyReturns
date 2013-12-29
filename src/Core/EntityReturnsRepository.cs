using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core
{
    public class EntityReturnsRepository
    {
        private readonly IEntityDataRepository _entityDataRepository;
        private readonly IReturnsSeriesDataRepository _returnsSeriesDataRepository;

        public EntityReturnsRepository(
            IEntityDataRepository entityDataRepository,
            IReturnsSeriesDataRepository returnsSeriesDataRepository)
        {
            _returnsSeriesDataRepository = returnsSeriesDataRepository;
            _entityDataRepository = entityDataRepository;
        }

        public Entity[] GetEntitiesWithReturnSeries()
        {
            var entityDtos = _entityDataRepository.GetAllEntities();

            var allReturnSeriesDtos = GetReturnSeriesDtos(entityDtos);
            var allMonthlyReturnDtos = GetMonthlyReturns(allReturnSeriesDtos);

            var dtosSource = new EntityFactory(allReturnSeriesDtos, allMonthlyReturnDtos);

            return entityDtos
                .Select(dtosSource.CreateEntity)
                .ToArray();
        }

        private ReturnSeriesDto[] GetReturnSeriesDtos(
            EntityDto[] entityDtos)
        {
            var distinctEntityNumbers = entityDtos
                .Select(d => d.EntityNumber)
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

            var monthlyReturnDtos = _returnsSeriesDataRepository
                .GetMonthlyReturns(distinctReturnSeriesIds);

            return monthlyReturnDtos;
        }
    }
}