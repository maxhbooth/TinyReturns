using System;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DataRepositories;
using Dimensional.TinyReturns.Core.DateExtend;
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
                Action<ReturnSeriesDtoCollectionForTests> a)
            {
                _returnsSeriesDataRepositoryStub
                    .SetupGetReturnSeries(entityNumbers, a);
            }

            public void SetupGetMonthlyReturns(
                int[] returnSeriesIds,
                Action<MonthlyReturnDtoCollectionForTests> listAction)
            {
                _returnsSeriesDataRepositoryStub
                    .SetupGetMonthlyReturns(returnSeriesIds, listAction);
                
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
                .SetupGetReturnSeries(
                    new []{ 100 },
                    c => c
                        .AddNetOfFeesReturnSeries(1000, 100));

            var repository = testHelper.CreateEntityReturnsRepository();

            var results = repository.GetEntitiesWithReturnSeries();

            Assert.Equal(results[0].ReturnSeriesCount, 1);

            var allReturnSeries = results[0].GetAllReturnSeries();

            Assert.Equal(allReturnSeries[0].ReturnSeriesId, 1000);
            Assert.Equal(allReturnSeries[0].FeeType, FeeType.NetOfFees);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndMultipleReturnSeries()
        {
            var testHelper = new TestHelper();

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

            Assert.Equal(results[0].ReturnSeriesCount, 2);

            var allReturnSeries = results[0].GetAllReturnSeries();

            Assert.Equal(allReturnSeries[1].ReturnSeriesId, 1001);
            Assert.Equal(allReturnSeries[1].FeeType, FeeType.GrossOfFees);
        }

        [Fact]
        public void GetEntitiesWithReturnSeriesShouldMapSingleEntityAndSingleReturn()
        {
            var testHelper = new TestHelper();

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

            var allReturnSeries = results[0].GetAllReturnSeries();

            Assert.Equal(allReturnSeries[0].MonthlyReturnsCount, 1);

            var allMonthlyReturns = allReturnSeries[0].GetAllMonthlyReturns();

            Assert.Equal(allMonthlyReturns[0].ReturnSeriesId, 1000);
            Assert.Equal(allMonthlyReturns[0].MonthYear, new MonthYear(2000, 1));
            Assert.Equal(allMonthlyReturns[0].ReturnValue, 0.1m);
        }

    }
}