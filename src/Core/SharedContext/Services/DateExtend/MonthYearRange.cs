using System;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend
{
    public class MonthYearRange
    {
        protected bool Equals(MonthYearRange other)
        {
            return Equals(StartMonth, other.StartMonth) && Equals(EndMonth, other.EndMonth);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MonthYearRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((StartMonth != null ? StartMonth.GetHashCode() : 0) * 397) ^ (EndMonth != null ? EndMonth.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MonthYearRange left, MonthYearRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonthYearRange left, MonthYearRange right)
        {
            return !Equals(left, right);
        }

        public static MonthYearRange CreateForEndMonthAndMonthsBack(
            MonthYear endMonthYear, int monthsBack)
        {
            var startMonthYear = endMonthYear.AddMonths((monthsBack * -1) + 1);

            var forEndMonthAndMonthsBack = new MonthYearRange(startMonthYear, endMonthYear);

            return forEndMonthAndMonthsBack;
        }
        public MonthYearRange()
        {
        }

        public MonthYearRange(
            int startYear,
            int startMonth,
            int endYear,
            int endMonth)
        {
            StartMonth = new MonthYear(startYear, startMonth);
            EndMonth = new MonthYear(endYear, endMonth);
        }

        public MonthYearRange(
            MonthYear startMonth,
            MonthYear endMonth)
        {
            StartMonth = startMonth;
            EndMonth = endMonth;
        }

        public MonthYearRange(
            DateTime startDate,
            DateTime endDate)
        {
            StartMonth = new MonthYear(startDate);
            EndMonth = new MonthYear(endDate);
        }

        public MonthYear StartMonth { get; set; }
        public MonthYear EndMonth { get; set; }

        public DateTime FirstDay
        {
            get { return StartMonth.FirstDayOfMonth; }
        }

        public DateTime LastDay
        {
            get { return EndMonth.LastDayOfMonth; }
        }

        public string GetFullMonthNameAndYear()
        {
            return string.Format("{0} to {1}", StartMonth.GetFullMonthNameAndYear(), EndMonth.GetFullMonthNameAndYear());
        }

        public DateTime LastDayOfStartMonth
        {
            get { return StartMonth.LastDayOfMonth; }
        }

        public DateTime LastDayOfEndMonth
        {
            get { return EndMonth.LastDayOfMonth; }
        }

        public int NumberOfMonthsInRange
        {
            get { return EndMonth - StartMonth + 1; }
        }

        public bool IsWithinLastDaysOfMonthsInclusively(
            DateTime d)
        {
            return d >= LastDayOfStartMonth && d <= LastDayOfEndMonth;
        }

        public void ForEachMonthInRange(Action<MonthYear> monthYearAction)
        {
            var monthCounter = new MonthYear(StartMonth);

            while (monthCounter <= EndMonth)
            {
                monthYearAction(new MonthYear(monthCounter));
                monthCounter++;
            }
        }

        public bool IsMonthInRange(
            MonthYear monthYear)
        {
            return StartMonth <= monthYear && EndMonth >= monthYear;
        }

        public void ForEachLastMonthInQuarterInRange(Action<MonthYear> monthYearAction)
        {
            var monthCounter = new MonthYear(StartMonth);

            while (monthCounter <= EndMonth)
            {
                var monthYear = new MonthYear(monthCounter);

                if (monthYear.IsLastMonthOfQuarter)
                    monthYearAction(monthYear);

                monthCounter++;
            }
        }

//        public void ForEachFullQuarter(Action<QuarterYear> quarterYearAction)
//        {
//            ForEachMonthInRange(my =>
//            {
//                if (my.IsLastMonthOfQuarter)
//                {
//                    var twoMonthBack = my.AddMonths(-2);
//
//                    if (IsMonthInRange(twoMonthBack))
//                    {
//                        var quarterYear = new QuarterYear(my);
//                        quarterYearAction(quarterYear);
//                    }
//                }
//            });
//        }
    }
}