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
            //var QuarterMonth = (endMonth.Month) % 3;    
            //var diffMonths = endMonth.Month - QuarterMonth + 1; 

            var quarterMonth = 0;

            if(endMonth.Month <= 3)
            {
                quarterMonth = 1;
            }
            else if (endMonth.Month <= 6 )
            {
                quarterMonth = 4;
            }
            else if (endMonth.Month <= 9)
            {
                quarterMonth = 7;
            }
            else
            {
                quarterMonth = 10;
            }

            var diffMonths = endMonth.Month - quarterMonth + 1;

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