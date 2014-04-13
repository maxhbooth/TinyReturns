using System;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class MonthlyReturnSeriesForStandardDeviationTests
    {
        private readonly MonthlyReturnSeries _returnSeries;

        public MonthlyReturnSeriesForStandardDeviationTests()
        {
            _returnSeries = new MonthlyReturnSeries();
        }

        [Fact]
        public void CalculationStandardDevationShouldReturnSomeValue()
        {
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 12);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            Assert.NotNull(result);
        }

        [Fact]
        public void CalculationStandardDevationShouldReturnErrorWhenNoMonthsAreFound()
        {
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 12);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            var expected = ReturnResult.CreateWithError("Could not find the correct number months in the given range.");

            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void CalculateStandardDeviationShouldErrorWhenNotEnoughMonthsFoundInRange()
        {
            _returnSeries.AddReturn(new MonthYear(2014, 1), 0.3m);
            _returnSeries.AddReturn(new MonthYear(2014, 2), 0.4m);
            
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 3);
            
            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            var expected = ReturnResult.CreateWithError("Could not find the correct number months in the given range.");

            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void CalculateStandardDeviationShouldErrorWhenOneMonthOrLessIsInRange()
        {
            var monthYearRange = new MonthYearRange(2014, 5, 2014, 5);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            var expected = ReturnResult.CreateWithError("Given range much have more than one month to calculate standard deviation.");

            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void CalculateStandardDeviationShouldReturnCorrectValueOnTwoMonths()
        {
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 2);

            _returnSeries.AddReturn(new MonthYear(2014, 1), 0.3m);
            _returnSeries.AddReturn(new MonthYear(2014, 2), 0.4m);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            Assert.False(result.HasError);
            Assert.Equal(result.Value, 0.070711m, 5);
        }

        [Fact]
        public void CalculateStandardDeviationShouldReturnCalculationOnTwoMonths()
        {
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 2);

            _returnSeries.AddReturn(new MonthYear(2014, 1), 0.3m);
            _returnSeries.AddReturn(new MonthYear(2014, 2), 0.4m);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            Assert.False(result.HasError);
            Assert.Equal("Sqrt((0.3 - ((0.3 + 0.4) / 2))^2 + (0.4 - ((0.3 + 0.4) / 2))^2 / (2 - 1))", result.Calculation);
        }

        [Fact]
        public void CalculateStandardDeviationShouldReturnCalculationOnThreeMonths()
        {
            var monthYearRange = new MonthYearRange(2014, 1, 2014, 3);

            _returnSeries.AddReturn(new MonthYear(2014, 1), 0.3m);
            _returnSeries.AddReturn(new MonthYear(2014, 2), 0.4m);
            _returnSeries.AddReturn(new MonthYear(2014, 3), 0.5m);

            var result = _returnSeries.CalculateStandardDeviation(monthYearRange);

            var expected = new ReturnResult();

            expected.SetValue(0.1m, "Sqrt((0.3 - (((0.3 + 0.4) + 0.5) / 3))^2 + (0.4 - (((0.3 + 0.4) + 0.5) / 3))^2 + (0.5 - (((0.3 + 0.4) + 0.5) / 3))^2 / (3 - 1))");

            expected.ShouldBeSameAs(result);
        }
    }
}