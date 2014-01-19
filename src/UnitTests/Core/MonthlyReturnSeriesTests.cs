﻿using Dimensional.TinyReturns.Core;
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
        public void CalculateReturnShouldErrorResultWhenSingleMonthNotFound()
        {
            var month = new MonthYear(2013, 1);
            var result = _returnSeries.CalculateReturn(month, 1);

            var expectedResult = ReturnResult.CreateWithError("Could not find return(s) for month(s).");

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldReturnValueWhenMonthFound()
        {
            var month = new MonthYear(2013, 1);

            _returnSeries.AddReturn(month, 0.03m);
            var result = _returnSeries.CalculateReturn(month, 1);

            var expectedResult = ReturnResult.CreateWithValue(0.03m, "0.03 = 0.03");

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldApplyGeoMetricLinkingWhenGivenMultipleMonths()
        {
            var month = new MonthYear(2013, 1);
            var previousMonth = month.AddMonths(-1);

            _returnSeries.AddReturn(previousMonth, 0.2m);
            _returnSeries.AddReturn(month, 0.1m);

            var result = _returnSeries.CalculateReturn(month, 2);

            var expectedResult = ReturnResult.CreateWithValue(0.32m, "((1 + 0.2) * (1 + 0.1) - 1) = 0.32");

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldErrorWhenMonthMissing()
        {
            var month = new MonthYear(2013, 1);
            var previousMonth = month.AddMonths(-1);
            var previousMonth2 = previousMonth.AddMonths(-1);

            _returnSeries.AddReturn(previousMonth2, 0.3m);
            //_returnSeries.AddReturn(previousMonth, 0.2m); -- Missing
            _returnSeries.AddReturn(month, 0.1m);

            var result = _returnSeries.CalculateReturn(month, 2);

            var expectedResult = ReturnResult.CreateWithError("Could not find a complete / unique set of months.");

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldErrorWhenMonthIsDuplicated()
        {
            var month = new MonthYear(2013, 1);
            var previousMonth = month.AddMonths(-1);
            var previousMonth2 = previousMonth.AddMonths(-1);

            _returnSeries.AddReturn(previousMonth2, 0.3m);
            _returnSeries.AddReturn(previousMonth, 0.2m);
            _returnSeries.AddReturn(previousMonth, 0.2m);
            _returnSeries.AddReturn(month, 0.1m);

            var result = _returnSeries.CalculateReturn(month, 2);

            var expectedResult = ReturnResult.CreateWithError("Could not find a complete / unique set of months.");

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldAnnualizeGivenMonthsGreaterThan12()
        {
            var month = new MonthYear(2013, 12);
            _returnSeries.AddReturn(month, 0.1m);
            _returnSeries.AddReturn(month.AddMonths(-1), 0.2m);
            _returnSeries.AddReturn(month.AddMonths(-2), 0.3m);
            _returnSeries.AddReturn(month.AddMonths(-3), 0.4m);
            _returnSeries.AddReturn(month.AddMonths(-4), 0.5m);
            _returnSeries.AddReturn(month.AddMonths(-5), 0.6m);
            _returnSeries.AddReturn(month.AddMonths(-6), 0.7m);
            _returnSeries.AddReturn(month.AddMonths(-7), 0.8m);
            _returnSeries.AddReturn(month.AddMonths(-8), 0.9m);
            _returnSeries.AddReturn(month.AddMonths(-9), 0.10m);
            _returnSeries.AddReturn(month.AddMonths(-10), 0.11m);
            _returnSeries.AddReturn(month.AddMonths(-11), 0.12m);
            _returnSeries.AddReturn(month.AddMonths(-12), 0.13m);
            _returnSeries.AddReturn(month.AddMonths(-13), 0.14m);

            var result = _returnSeries.CalculateReturn(month, 13);

            string expectedCalculation = @"((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + 0.1) * (1 + 0.2) - 1)) * (1 + 0.3) - 1)) * (1 + 0.4) - 1)) * (1 + 0.5) - 1)) * (1 + 0.6) - 1)) * (1 + 0.7) - 1)) * (1 + 0.8) - 1)) * (1 + 0.9) - 1)) * (1 + 0.10) - 1)) * (1 + 0.11) - 1)) * (1 + 0.12) - 1)) * (1 + 0.13) - 1) = 50.80166493428326400"
                + System.Environment.NewLine + @"((1 + 50.80166493428326400) ^ (12 * 1 / 13)) - 1 = 37.2358830610463";

            var expectedResult = ReturnResult.CreateWithValue(37.2358830610463m, expectedCalculation);

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        [Fact]
        public void CalculateReturnShouldNotAnnualizeWhenGivenDoNotAnnualizeOption()
        {
            var month = new MonthYear(2013, 12);
            _returnSeries.AddReturn(month, 0.1m);
            _returnSeries.AddReturn(month.AddMonths(-1), 0.2m);
            _returnSeries.AddReturn(month.AddMonths(-2), 0.3m);
            _returnSeries.AddReturn(month.AddMonths(-3), 0.4m);
            _returnSeries.AddReturn(month.AddMonths(-4), 0.5m);
            _returnSeries.AddReturn(month.AddMonths(-5), 0.6m);
            _returnSeries.AddReturn(month.AddMonths(-6), 0.7m);
            _returnSeries.AddReturn(month.AddMonths(-7), 0.8m);
            _returnSeries.AddReturn(month.AddMonths(-8), 0.9m);
            _returnSeries.AddReturn(month.AddMonths(-9), 0.10m);
            _returnSeries.AddReturn(month.AddMonths(-10), 0.11m);
            _returnSeries.AddReturn(month.AddMonths(-11), 0.12m);
            _returnSeries.AddReturn(month.AddMonths(-12), 0.13m);
            _returnSeries.AddReturn(month.AddMonths(-13), 0.14m);

            var result = _returnSeries.CalculateReturn(month, 13, AnnualizeActionEnum.DoNotAnnualize);

            string expectedCalculation = @"((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + ((1 + 0.1) * (1 + 0.2) - 1)) * (1 + 0.3) - 1)) * (1 + 0.4) - 1)) * (1 + 0.5) - 1)) * (1 + 0.6) - 1)) * (1 + 0.7) - 1)) * (1 + 0.8) - 1)) * (1 + 0.9) - 1)) * (1 + 0.10) - 1)) * (1 + 0.11) - 1)) * (1 + 0.12) - 1)) * (1 + 0.13) - 1) = 50.80166493428326400";

            var expectedResult = ReturnResult.CreateWithValue(50.80166493428326400m, expectedCalculation);

            Assert.NotNull(result);
            AssetResultMatches(result, expectedResult);
        }

        private void AssetResultMatches(
            ReturnResult result, ReturnResult expected)
        {
            Assert.Equal(result.Value, expected.Value);
            Assert.Equal(result.Calculation, expected.Calculation);
            Assert.Equal(result.HasError, expected.HasError);
            Assert.Equal(result.ErrorMessage, expected.ErrorMessage);
        }
    }
}