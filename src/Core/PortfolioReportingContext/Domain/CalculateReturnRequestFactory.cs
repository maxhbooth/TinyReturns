﻿using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class CalculateReturnRequestFactory
    {
        public static CalculateReturnRequest OneMonth(
            MonthYear endMonth)
        {
            return new CalculateReturnRequest(
                endMonth, 1);
        }

        public static CalculateReturnRequest ThreeMonth(
            MonthYear endMonth)
        {
            return new CalculateReturnRequest(
                endMonth, 3);
        }

        public static CalculateReturnRequest SixMonth(
            MonthYear endMonth)
        {
            return new CalculateReturnRequest(
                endMonth, 6);
        }

        public static CalculateReturnRequest YearToDate(
            MonthYear endMonth)
        {
            var firstMonthOfYear = new MonthYear(endMonth.Year, 1);

            var diffMonths = endMonth - firstMonthOfYear + 1;

            return new CalculateReturnRequest(
                endMonth, diffMonths);
        }

        public static CalculateReturnRequest QuarterToDate(
            MonthYear endMonth)
        {



            var diffMonths = endMonth - endMonth.getQuarterMonth() + 1;

            return new CalculateReturnRequest(
                endMonth,diffMonths);
        }

        public static CalculateReturnRequest TwelveMonth(MonthYear endMonth)
        {
            return new CalculateReturnRequest(
                endMonth, 12);
        }
    }
}