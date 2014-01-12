using System;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DataRepositories;
using Dimensional.TinyReturns.Core.DateExtend;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class EntityReturnsRepositoryTests
    {
        private class EntityReturnsRepositoryTestHelper
        {
            private readonly EntityDataRepositoryStub _entityDataRepositoryStub;
            private readonly ReturnsSeriesDataRepositoryStub _returnsSeriesDataRepositoryStub;
            private readonly MonthlyReturnsDataRepositoryStub _monthlyReturnsDataRepositoryStub;

            public EntityReturnsRepositoryTestHelper()
            {
                _entityDataRepositoryStub = new EntityDataRepositoryStub();
                _returnsSeriesDataRepositoryStub = new ReturnsSeriesDataRepositoryStub();
                _monthlyReturnsDataRepositoryStub = new MonthlyReturnsDataRepositoryStub();
            }

            public EntityReturnsRepository CreateEntityReturnsRepository()
            {
                var repository = new EntityReturnsRepository(
                    _entityDataRepositoryStub,
                    _returnsSeriesDataRepositoryStub,
                    _monthlyReturnsDataRepositoryStub);

                return repository;
            }

            public void SetupGetAllEntities(
                Action<EntityDataRepositoryStub.EntityDtoCollectionForTest> a)
            {
                _entityDataRepositoryStub.SetupGetAllEntities(a);
            }

            public void SetupGetReturnSeries(
                int[] entityNumbers,
                Action<ReturnSeriesDtoCollectionForTests> a)
            {
                _returnsSeriesDataRepositoryStub
                    .SetupGetReturnSeries(entityNumbers, a);
            }

            public void SetupGetMonthlyReturns(
                int[] returnSeriesIds,
                Action<MonthlyReturnDtoCollectionForTests> listAction)
            {
                _monthlyReturnsDataRepositoryStub
                    .SetupGetMonthlyReturns(returnSeriesIds, listAction);
            }
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntity()
        {
            var testHelper = new EntityReturnsRepositoryTestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.Equal(results.Length, 1);

            var expectedEntity = new Entity()
            {
                EntityNumber = 100,
                Name = "Port100",
                EntityType = EntityType.Portfolio
            };

            Assert.Equal(results[0], expectedEntity);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndSingleReturnSeries()
        {
            var testHelper = new EntityReturnsRepositoryTestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            testHelper
                .SetupGetReturnSeries(
                    new []{ 100 },
                    c => c.AddNetOfFeesReturnSeries(1000, 100));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            var expectedEntity = new Entity()
            {
                EntityNumber = 100,
                Name = "Port100",
                EntityType = EntityType.Portfolio
            };

            expectedEntity.AddReturnSeries(new ReturnSeries() { ReturnSeriesId = 1000, FeeType = FeeType.NetOfFees } );

            Assert.Equal(results[0], expectedEntity);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndMultipleReturnSeries()
        {
            var testHelper = new EntityReturnsRepositoryTestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            testHelper
                .SetupGetReturnSeries(
                    new[] { 100 },
                    c => c
                        .AddNetOfFeesReturnSeries(1000, 100)
                        .AddNetOfGrossReturnSeries(1001, 100));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            var expectedEntity = new Entity()
            {
                EntityNumber = 100,
                Name = "Port100",
                EntityType = EntityType.Portfolio
            };

            expectedEntity.AddReturnSeries(new ReturnSeries() { ReturnSeriesId = 1000, FeeType = FeeType.NetOfFees });
            expectedEntity.AddReturnSeries(new ReturnSeries() { ReturnSeriesId = 1001, FeeType = FeeType.GrossOfFees });

            Assert.Equal(results[0], expectedEntity);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndSingleReturn()
        {
            var testHelper = new EntityReturnsRepositoryTestHelper();

            testHelper
                .SetupGetAllEntities(c => c.AddPortfolio(100, "Port100"));

            testHelper.SetupGetReturnSeries(
                new[] { 100 },
                c => c.AddNetOfFeesReturnSeries(1000, 100));

            testHelper
                .SetupGetMonthlyReturns(new []{ 1000 },
                c => c.Add(new MonthlyReturnDto() { ReturnSeriesId = 1000, Year = 2000, Month = 1, ReturnValue = 0.1m } ));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            var expectedEntity = new Entity()
            {
                EntityNumber = 100,
                Name = "Port100",
                EntityType = EntityType.Portfolio
            };

            var returnSeries = new ReturnSeries() { ReturnSeriesId = 1000, FeeType = FeeType.NetOfFees };
            expectedEntity.AddReturnSeries(returnSeries);

            returnSeries.AddReturn(new MonthYear(2000, 1), 0.1m);
            
            Assert.Equal(results[0], expectedEntity);
        }

    }
}