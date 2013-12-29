using System;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class EntityReturnsRepositoryTests
    {
        private class TestHelper
        {
            private readonly EntityDataRepositoryStub _entityDataRepositoryStub;
            private readonly ReturnsSeriesDataRepositoryStub _returnsSeriesDataRepositoryStub;

            public TestHelper()
            {
                _entityDataRepositoryStub = new EntityDataRepositoryStub();
                _returnsSeriesDataRepositoryStub = new ReturnsSeriesDataRepositoryStub();
            }

            public EntityReturnsRepository CreateEntityReturnsRepository()
            {
                var repository = new EntityReturnsRepository(
                    _entityDataRepositoryStub, _returnsSeriesDataRepositoryStub);                

                return repository;
            }

            public void SetupGetAllEntities(
                Action<EntityDataRepositoryStub.EntityDtoCollectionForTest> a)
            {
                _entityDataRepositoryStub.SetupGetAllEntities(a);
            }

            public void SetupGetReturnSeries(
                int[] entityNumbers,
                Action<ReturnsSeriesDataRepositoryStub.ReturnSeriesDtoCollection> listAction)
            {
                _returnsSeriesDataRepositoryStub
                    .SetupGetReturnSeries(entityNumbers, listAction);
            }
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntity()
        {
            var testHelper = new TestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.Equal(results.Length, 1);

            Assert.Equal(results[0].EntityNumber, 100);
            Assert.Equal(results[0].Name, "Port100");
            Assert.Equal(results[0].EntityType, EntityType.Portfolio);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndSingleReturnSeries()
        {
            var testHelper = new TestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            testHelper
                .SetupGetReturnSeries(new []{ 100 },
                    c => c.AddNetOfFeesReturnSeries(1000, 100));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.Equal(results[0].ReturnSeries.Length, 1);
            Assert.Equal(results[0].ReturnSeries[0].ReturnSeriesId, 1000);
            Assert.Equal(results[0].ReturnSeries[0].FeeType, FeeType.NetOfFees);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndMultipuleReturnSeries()
        {
            var testHelper = new TestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            testHelper
                .SetupGetReturnSeries(new[] { 100 }, c => c
                    .AddNetOfFeesReturnSeries(1000, 100)
                    .AddNetOfGrossReturnSeries(1001, 100));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.Equal(results[0].ReturnSeries.Length, 2);
            Assert.Equal(results[0].ReturnSeries[1].ReturnSeriesId, 1001);
            Assert.Equal(results[0].ReturnSeries[1].FeeType, FeeType.GrossOfFees);
        }
    }
}