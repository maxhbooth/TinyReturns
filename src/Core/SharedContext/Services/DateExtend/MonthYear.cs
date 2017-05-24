using System;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend
{
    public class MonthYear : IComparable<MonthYear>
    {
        private const int MaxMonth = 12;
        private const int MinMonth = 1;

        public static MonthYear CreateMonthYearFrom(
            IMonthAndYear monthAndYear)
        {
            var monthYear = new MonthYear(
                monthAndYear.Year,
                monthAndYear.Month);

            return monthYear;
        }

        public MonthYear(int year, int month)
        {
            Month = month;
            Year = year;

            Validate();
        }

        public MonthYear(DateTime monthDate)
        {
            Month = monthDate.Month;
            Year = monthDate.Year;

            Validate();
        }

        public MonthYear(MonthYear monthYear)
        {
            Month = monthYear.Month;
            Year = monthYear.Year;

            Validate();
        }

        public int Month { get; private set; }
        public int Year { get; private set; }

        public DateTime LastDayOfMonth
        {
            get
            {
                return FirstDayOfMonth.GetLastDayOfMonth();
            }
        }

        public DateTime LastDayOfPeviousMonth
        {
            get
            {
                var prevMonth = AddMonths(-1);
                return prevMonth.LastDayOfMonth;
            }
        }

        public MonthYear AddMonths(int months)
        {
            var dateTime = new DateTime(Year, Month, 1);

            var mo = dateTime.AddMonths(months);

            return new MonthYear(mo);
        }

        public DateTime FirstDayOfMonth
        {
            get { return new DateTime(Year, Month, 1); }
        }

        public bool IsLastMonthOfQuarter
        {
            get
            {
                if ((Month == 3) || (Month == 6) || (Month == 9) || (Month == 12))
                    return true;

                return false;
            }
        }

        public static bool operator ==(MonthYear lhs, MonthYear rhs)
        {
            if (((object)lhs == null) && ((object)rhs == (null)))
                return true;

            if (((object)lhs == null) || ((object)rhs == (null)))
                return false;

            return ((lhs.Month == rhs.Month) && (lhs.Year == rhs.Year));
        }

        public static bool operator !=(MonthYear lhs, MonthYear rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(MonthYear other)
        {
            if (Year < other.Year)
                return -1;

            if (Year > other.Year)
                return 1;

            if (Month < other.Month)
                return -1;

            if (Month > other.Month)
                return 1;

            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is MonthYear)
            {
                return (this == (MonthYear)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int h;

            h = Year << 4; // Multiply by 16

            h += Month;

            return h;
        }

        public static bool operator <(MonthYear lhs, MonthYear rhs)
        {
            if (((object)lhs == null) || ((object)rhs == null))
                return false;

            if (lhs.Year < rhs.Year)
                return true;

            if (lhs.Year == rhs.Year)
            {
                return (lhs.Month < rhs.Month);
            }

            return false;
        }

        public static bool operator <=(MonthYear lhs, MonthYear rhs)
        {
            if (lhs < rhs)
                return true;

            return (lhs == rhs);
        }

        public static bool operator >(MonthYear lhs, MonthYear rhs)
        {
            if (((object)lhs == null) || ((object)rhs == null))
                return false;

            if (lhs.Year > rhs.Year)
                return true;

            if (lhs.Year == rhs.Year)
            {
                return (lhs.Month > rhs.Month);
            }

            return false;
        }

        public static bool operator >=(MonthYear lhs, MonthYear rhs)
        {
            if (lhs > rhs)
                return true;

            return (lhs == rhs);
        }

        public static MonthYear operator ++(MonthYear moy)
        {
            int m;

            m = moy.Month;

            if (++m > MaxMonth)
            {
                m = MinMonth;

                moy.Year++;
            }

            moy.Month = m;

            return moy;
        }

        public static MonthYear operator --(MonthYear monthYear)
        {
            int m;

            m = monthYear.Month;

            if (--m < MinMonth)
            {
                m = MaxMonth;
                monthYear.Year--;
            }

            monthYear.Month = m;

            return monthYear;
        }

        public static MonthYear operator +(MonthYear monthYear, int add)
        {
            var resultMonthYear = new MonthYear(monthYear);

            resultMonthYear.Month += add;

            while (resultMonthYear.Month < MinMonth)
            {
                resultMonthYear.Month += MaxMonth;
                resultMonthYear.Year--;
            }

            while (resultMonthYear.Month > MaxMonth)
            {
                resultMonthYear.Month -= MaxMonth;
                resultMonthYear.Year++;
            }

            return resultMonthYear;
        }

        public static MonthYear operator -(MonthYear monthYear, int sub)
        {
            return (monthYear + (-sub));
        }

        public static int operator -(MonthYear lhs, MonthYear rhs)
        {
            return lhs.GetMonthNumber() - rhs.GetMonthNumber();
        }

        public int GetMonthNumber()
        {
            return Year * 12 + (Month - 1);
        }

        private void Validate()
        {
            if (Month < 1)
                throw new Exception("The given month is less than 1.");

            if (Year < 1)
                throw new Exception("The given year is less than 1.");

            if (Month > 12)
                throw new Exception("The given month is greater than 12.");
        }

        public string ToSmallString()
        {
            return Month + "/" + Year.ToString().Substring(2, 2);
        }

        public int GetQuarterNumber()
        {
            if (Month <= 3)
                return 1;

            if (Month <= 6)
                return 2;

            if (Month <= 9)
                return 3;

            if (Month <= 12)
                return 4;

            return 0;
        }

        public string GetFullMonthNameAndYear()
        {
            var dateTime = new DateTime(Year, Month, 1);
            return dateTime.ToString("MMMM yyyy");
        }

        public string GetSixDigit()
        {
            var dateTime = new DateTime(Year, Month, 1);
            return dateTime.ToString("yyyyMM");
        }

        public bool IsDateWithInMonth(DateTime dateTime)
        {
            return (dateTime >= FirstDayOfMonth) && (dateTime < AddMonths(1).FirstDayOfMonth);
        }

        public int GetUniqueNumber()
        {
            return Month + (Year * 12);
        }

        public static MonthYear CreateFromUniqueNumber(int uniqueNumber)
        {
            int month = uniqueNumber % 12;
            var year = uniqueNumber / 12;

            if (month <= 0)
            {
                month = 12;
                year--;
            }
            return new MonthYear(year, month);
        }

        public bool IsOnOrPassedFirstQuarter
        {
            get { return Month >= 3; }
        }

        public bool IsOnOrPassedSecondQuarter
        {
            get { return Month >= 6; }
        }

        public bool IsOnOrPassedThirdQuarter
        {
            get { return Month >= 9; }
        }

        public bool IsOnFourthQuarter
        {
            get { return Month >= 12; }
        }
    }
}