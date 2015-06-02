using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DataRepositories;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core.DataRepository
{
    public class ReturnSeriesDataRepositoryTests : DatabaseTestBase
    {
        private readonly IReturnsSeriesDataGateway _returnsSeriesDataGateway;
        private IMonthlyReturnsDataGateway _monthlyReturnsDataGateway;

        public ReturnSeriesDataRepositoryTests()
        {
            _returnsSeriesDataGateway = MasterFactory.GetReturnsSeriesRepository();
            _monthlyReturnsDataGateway = MasterFactory.GetMonthlyReturnsDataRepository();
        }

        [Fact]
        public void ShouldReadAndWriteReturnSeries()
        {
            var newReturnsSeries = InsertTestReturnSeries();

            var savedReturnSeries = _returnsSeriesDataGateway.GetReturnSeries(newReturnsSeries.ReturnSeriesId);

            AssertReturnSeriesRecordIsValid(savedReturnSeries, newReturnsSeries);

            _returnsSeriesDataGateway.DeleteReturnSeries(newReturnsSeries.ReturnSeriesId);
        }

        [Fact]
        public void ShouldReadAndWriteMonthlyReturns()
        {
            var newReturnsSeries = InsertTestReturnSeries();

            var testMonthlyReturns = CreateTestMonthlyReturns(newReturnsSeries);

            _monthlyReturnsDataGateway.InsertMonthlyReturns(testMonthlyReturns);

            var savedMonthlyReturns = _monthlyReturnsDataGateway.GetMonthlyReturns(newReturnsSeries.ReturnSeriesId);

            AssertMonthlyReturnsAreValid(savedMonthlyReturns, newReturnsSeries.ReturnSeriesId);

            _monthlyReturnsDataGateway.DeleteMonthlyReturns(newReturnsSeries.ReturnSeriesId);
            _returnsSeriesDataGateway.DeleteReturnSeries(newReturnsSeries.ReturnSeriesId);
        }

        private void AssertMonthlyReturnsAreValid(
            MonthlyReturnDto[] savedMonthlyReturns,
            int returnSeriesId)
        {
            Assert.Equal(savedMonthlyReturns.Length, 3);

            var target = savedMonthlyReturns.FirstOrDefault(r =>
                r.Month == 1 && r.Year == 2000 && r.ReturnSeriesId == returnSeriesId);

            Assert.NotNull(target);

            Assert.Equal(target.ReturnValue, 0.1m);
        }

        private void AssertReturnSeriesRecordIsValid(
            ReturnSeriesDto savedReturnSeries,
            ReturnSeriesDto expectedReturnSeries)
        {
            Assert.NotNull(savedReturnSeries);

            Assert.Equal(savedReturnSeries.ReturnSeriesId, expectedReturnSeries.ReturnSeriesId);
            Assert.Equal(savedReturnSeries.InvestmentVehicleNumber, expectedReturnSeries.InvestmentVehicleNumber);
            Assert.Equal(savedReturnSeries.FeeTypeCode, expectedReturnSeries.FeeTypeCode);
        }

        private static MonthlyReturnDto[] CreateTestMonthlyReturns(
            ReturnSeriesDto returnSeries)
        {
            var monthlyReturnList = new List<MonthlyReturnDto>();

            monthlyReturnList.Add(new MonthlyReturnDto()
            {
                Year = 2000,
                Month = 1,
                ReturnValue = 0.1m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });
            monthlyReturnList.Add(new MonthlyReturnDto()
            {
                Year = 2000,
                Month = 2,
                ReturnValue = 0.2m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });
            monthlyReturnList.Add(new MonthlyReturnDto()
            {
                Year = 2000,
                Month = 3,
                ReturnValue = 0.3m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });

            var monthlyReturns = monthlyReturnList.ToArray();
            return monthlyReturns;
        }

        private ReturnSeriesDto InsertTestReturnSeries()
        {
            var returnSeries = new ReturnSeriesDto();

            returnSeries.InvestmentVehicleNumber = 100;
            returnSeries.FeeTypeCode = 'N';

            var newId = _returnsSeriesDataGateway.InsertReturnSeries(returnSeries);

            returnSeries.ReturnSeriesId = newId;

            return returnSeries;
        }
    }
}
