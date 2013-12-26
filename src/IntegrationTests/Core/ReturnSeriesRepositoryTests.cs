using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core
{
    public class ReturnSeriesRepositoryTests : DatabaseTestBase
    {
        private const string TestSeriesDescription = "Test Series 999";

        [Fact]
        public void ShouldReadAndWriteReturnSeries()
        {
            var tinyReturnsDatabase = MasterFactory.GetReturnsSeriesRepository();

            var returnSeries = InsertTestReturnSeries(tinyReturnsDatabase);

            var savedReturnSeries = tinyReturnsDatabase.GetReturnSeries(returnSeries.ReturnSeriesId);

            AssertReturnSeriesRecordIsValid(savedReturnSeries, returnSeries);

            tinyReturnsDatabase.DeleteReturnSeries(returnSeries.ReturnSeriesId);
        }

        [Fact]
        public void ShouldReadAndWriteMonthlyReturns()
        {
            var tinyReturnsDatabase = MasterFactory.GetReturnsSeriesRepository();

            var returnSeries = InsertTestReturnSeries(tinyReturnsDatabase);

            var monthlyReturns = CreateTestMonthlyReturns(returnSeries);

            tinyReturnsDatabase.InsertMonthlyReturns(monthlyReturns);
            var savedMonthlyReturns = tinyReturnsDatabase.GetMonthlyReturns(returnSeries.ReturnSeriesId);

            AssertMonthlyReturnsAreValid(savedMonthlyReturns, returnSeries.ReturnSeriesId);

            tinyReturnsDatabase.DeleteMonthlyReturns(returnSeries.ReturnSeriesId);
            tinyReturnsDatabase.DeleteReturnSeries(returnSeries.ReturnSeriesId);
        }

        private void AssertMonthlyReturnsAreValid(
            MonthlyReturn[] savedMonthlyReturns,
            int returnSeriesId)
        {
            Assert.Equal(savedMonthlyReturns.Length, 3);

            var target = savedMonthlyReturns.FirstOrDefault(r =>
                r.Month == 1 && r.Year == 2000 && r.ReturnSeriesId == returnSeriesId);

            Assert.NotNull(target);

            Assert.Equal(target.ReturnValue, 0.1m);
        }

        private void AssertReturnSeriesRecordIsValid(ReturnSeries savedReturnSeries, ReturnSeries returnSeries)
        {
            Assert.NotNull(savedReturnSeries);

            Assert.Equal(savedReturnSeries.ReturnSeriesId, returnSeries.ReturnSeriesId);
            Assert.Equal(savedReturnSeries.EntityNumber, returnSeries.EntityNumber);
            Assert.Equal(savedReturnSeries.Description, returnSeries.Description);
            Assert.Equal(savedReturnSeries.FeeTypeCode, returnSeries.FeeTypeCode);
        }

        private static MonthlyReturn[] CreateTestMonthlyReturns(ReturnSeries returnSeries)
        {
            var monthlyReturnList = new List<MonthlyReturn>();

            monthlyReturnList.Add(new MonthlyReturn()
            {
                Year = 2000,
                Month = 1,
                ReturnValue = 0.1m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });
            monthlyReturnList.Add(new MonthlyReturn()
            {
                Year = 2000,
                Month = 2,
                ReturnValue = 0.2m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });
            monthlyReturnList.Add(new MonthlyReturn()
            {
                Year = 2000,
                Month = 3,
                ReturnValue = 0.3m,
                ReturnSeriesId = returnSeries.ReturnSeriesId
            });

            var monthlyReturns = monthlyReturnList.ToArray();
            return monthlyReturns;
        }

        private ReturnSeries InsertTestReturnSeries(
            IReturnsSeriesRepository tinyReturnsDatabase)
        {
            var returnSeries = new ReturnSeries();

            returnSeries.EntityNumber = 100;
            returnSeries.Description = TestSeriesDescription;
            returnSeries.FeeTypeCode = 'N';

            var newId = tinyReturnsDatabase.InsertReturnSeries(returnSeries);

            returnSeries.ReturnSeriesId = newId;

            return returnSeries;
        }
    }
}
