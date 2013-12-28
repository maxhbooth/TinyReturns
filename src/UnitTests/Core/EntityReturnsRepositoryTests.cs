using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class EntityReturnsRepositoryTests
    {
        [Fact]
        public void GetEntitiesWithReturnSeriesShouldReturnValue()
        {
            var repository = new EntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.NotNull(results);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntity()
        {
            var repository = new EntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.NotNull(results);
        }
    }

    public class EntityReturnsRepository
    {
        public Entity[] GetEntitiesWithReturnSeries()
        {
            return new Entity[0];
        }
    }
}