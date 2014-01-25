using System.Linq;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.DataRepository
{
    public class InvestmentVehicleDataRepositoryTests : DatabaseTestBase
    {
        [Fact]
        public void GetAllEntitiesShouldReturnCorrectNumberOfEntities()
        {
            var entityDataRepository = MasterFactory.GetInvestmentVehicleDataRepository();

            var results = entityDataRepository.GetAllEntities();

            Assert.Equal(results.Length, 5);
        }

        [Fact]
        public void GetAllEntitiesShouldReturnMapAllColumnsToProperties()
        {
            var entityDataRepository = MasterFactory.GetInvestmentVehicleDataRepository();

            var results = entityDataRepository.GetAllEntities();

            var target = results.FirstOrDefault(r => r.InvestmentVehicleNumber == 100);

            Assert.NotNull(target);

            Assert.Equal(target.InvestmentVehicleTypeCode, 'P');
            Assert.Equal(target.Name, "Portfolio 100 - Large");
        }
    }
}