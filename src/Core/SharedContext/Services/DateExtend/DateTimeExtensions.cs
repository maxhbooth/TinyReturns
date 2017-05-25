using System;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            var returnDate = new DateTime(date.Year, date.Month, date.Day);

            returnDate = returnDate.AddMonths(1);

            returnDate = returnDate.AddDays(-(returnDate.Day));

            return returnDate;
        }
    }
}