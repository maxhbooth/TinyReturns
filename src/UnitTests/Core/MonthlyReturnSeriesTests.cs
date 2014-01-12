using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class MonthlyReturnSeriesTests
    {
        private readonly MonthlyReturnSeries _returnSeries;

        public MonthlyReturnSeriesTests()
        {
            _returnSeries = new MonthlyReturnSeries();
        }

        [Fact]
        public void GetMonthReturnShouldErrorResultWhenSingleMonthNotFound()
        {
            var month = new MonthYear(2013, 1);
            var result = _returnSeries.CalculateReturn(month);

            Assert.NotNull(result);
            Assert.True(result.HasError);
        }

        [Fact]
        public void GetMonthReturnShouldReturnValueWhenMonthFound()
        {
            var month = new MonthYear(2013, 1);

            _returnSeries.AddReturn(month, 0.03m);

            var result = _returnSeries.CalculateReturn(month);

            Assert.NotNull(result);
            Assert.False(result.HasError);
            Assert.Equal(result.Value, 0.03m);
        }
    }
}