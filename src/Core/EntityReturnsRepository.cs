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

            return entityDtos
                .Select(entityDto => CreateReturnSeries(entityDto, allReturnSeriesDtos))
                .ToArray();
        }

        private Entity CreateReturnSeries(
            EntityDto entityDto,
            ReturnSeriesDto[] allReturnSeriesDtos)
        {
            var entity = CreateEntityForDto(entityDto);

            var returnSeries = CreateReturnSeriesForEntity(allReturnSeriesDtos, entityDto);

            entity.ReturnSeries = returnSeries;

            return entity;
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

        private static Entity CreateEntityForDto(
            EntityDto entityDto)
        {
            return new Entity
            {
                EntityNumber = entityDto.EntityNumber,
                Name = entityDto.Name,
                EntityType = EntityType.FromCode(entityDto.EntityTypeCode)
            };
        }

        private ReturnSeries[] CreateReturnSeriesForEntity(
            ReturnSeriesDto[] returnSeriesDtos,
            EntityDto entityDto)
        {
            var seriesForEntity = returnSeriesDtos
                .Where(d => d.EntityNumber == entityDto.EntityNumber);

            var returnSeries = seriesForEntity
                .Select(CreateReturnSeries)
                .ToArray();

            return returnSeries;
        }

        private ReturnSeries CreateReturnSeries(
            ReturnSeriesDto r)
        {
            return new ReturnSeries()
            {
                ReturnSeriesId = r.ReturnSeriesId,
                FeeType = FeeType.FromCode(r.FeeTypeCode)
            };
        }
    }
}