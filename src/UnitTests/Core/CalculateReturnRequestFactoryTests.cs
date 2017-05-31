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

        [Fact]
        public void QuarterMonthShouldReturnRequestForOneBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 1);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 1
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForTwoBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 2);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 2
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForThreeBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 3);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForFourBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 4);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 1
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForFiveBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 5);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 2
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForSixBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 6);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForSevenBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 7);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 1
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForNineBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 9);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForNinceBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 9);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForTenBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 10);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 1
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForElvenBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 11);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 2
            };

            AssertRequestAreEqual(request, expected);
        }

        [Fact]
        public void QuarterMonthShouldReturnRequestForTwelveBeforeTheGivenEndDate()
        {
            var monthYear = new MonthYear(2000, 12);

            var request = CalculateReturnRequestFactory.QuarterToDate(monthYear);

            var expected = new CalculateReturnRequest()
            {
                AnnualizeAction = AnnualizeActionEnum.Annualize,
                EndMonth = monthYear,
                NumberOfMonths = 3
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