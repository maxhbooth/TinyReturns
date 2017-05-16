using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Xunit;
using Xunit.Extensions;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class CalculateReturnRequestFactoryTests
    {
        [Fact]
        public void OneMonthShouldReturnRequestForBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 5);

            var request = CalculateReturnRequestFactory.OneMonth(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 1
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void ThreeMonthShouldReturnRequestForThreeBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 5);

            var request = CalculateReturnRequestFactory.ThreeMonth(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void TwelveMonthShouldReturnRequestForThreeBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 5);

            var request = CalculateReturnRequestFactory.TwelveMonth(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 12
            };

            AssertRequestAreEqual(request, expected);
        }

        [Theory]
        [InlineData(2000, 5, 5)]
        [InlineData(2000, 1, 1)]
        [InlineData(2000, 12, 12)]
        public void YearToDateShouldReturnRequestFromFirstOfYearToEndMonth(
            int year,
            int month,
            int expectedNumberOfMonths)
        {
            var monthYear = new MonthYear(year, month);

            var request = CalculateReturnRequestFactory.YearToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = expectedNumberOfMonths
            };

            AssertRequestAreEqual(request, expected);
        }

        private void AssertRequestAreEqual(
            CalculateReturnRequest requestUnderTest,
            CalculateReturnRequest expected)
        {
            Assert.Equal(requestUnderTest.AnnualizeAction, expected.AnnualizeAction);
            Assert.Equal(requestUnderTest.EndMonth, expected.EndMonth);
            Assert.Equal(requestUnderTest.NumberOfMonths, expected.NumberOfMonths);
        }
    }
}