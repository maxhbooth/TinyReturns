using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DataRepository;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core
{
    public class ReturnSeriesRepositoryTests : DatabaseTestBase
    {
        private readonly IReturnsSeriesRepository _tinyReturnsDatabase;

        public ReturnSeriesRepositoryTests()
        {
            _tinyReturnsDatabase = MasterFactory.GetReturnsSeriesRepository();
        }

        [Fact]
        public void ShouldReadAndWriteReturnSeries()
        {
            var newReturnsSeries = InsertTestReturnSeries();

            var savedReturnSeries = _tinyReturnsDatabase.GetReturnSeries(newReturnsSeries.ReturnSeriesId);

            AssertReturnSeriesRecordIsValid(savedReturnSeries, newReturnsSeries);

            _tinyReturnsDatabase.DeleteReturnSeries(newReturnsSeries.ReturnSeriesId);
        }

        [Fact]
        public void ShouldReadAndWriteMonthlyReturns()
        {
            var newReturnsSeries = InsertTestReturnSeries();

            var testMonthlyReturns = CreateTestMonthlyReturns(newReturnsSeries);

            _tinyReturnsDatabase.InsertMonthlyReturns(testMonthlyReturns);

            var savedMonthlyReturns = _tinyReturnsDatabase.GetMonthlyReturns(newReturnsSeries.ReturnSeriesId);

            AssertMonthlyReturnsAreValid(savedMonthlyReturns, newReturnsSeries.ReturnSeriesId);

            _tinyReturnsDatabase.DeleteMonthlyReturns(newReturnsSeries.ReturnSeriesId);
            _tinyReturnsDatabase.DeleteReturnSeries(newReturnsSeries.ReturnSeriesId);
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
            Assert.Equal(savedReturnSeries.EntityNumber, expectedReturnSeries.EntityNumber);
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

            returnSeries.EntityNumber = 100;
            returnSeries.FeeTypeCode = 'N';

            var newId = _tinyReturnsDatabase.InsertReturnSeries(returnSeries);

            returnSeries.ReturnSeriesId = newId;

            return returnSeries;
        }
    }
}
