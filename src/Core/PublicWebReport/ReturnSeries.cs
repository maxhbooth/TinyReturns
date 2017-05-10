using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class ReturnSeries
    {
        private readonly MonthlyReturn[] _monthlyReturns;

        public ReturnSeries(
            int id,
            MonthlyReturn[] monthlyReturns)
        {
            Id = id;
            _monthlyReturns = monthlyReturns;
        }

        public int Id { get; }

        public decimal? GetMonthlyReturn(
            MonthYear monthYear)
        {
            var monthlyReturn = _monthlyReturns.FirstOrDefault(r => r.MonthYear.Equals(monthYear));

            if (monthlyReturn == null)
                return null;

            return monthlyReturn.Value;
        }

        public class MonthlyReturn
        {
            public MonthlyReturn(
                MonthYear monthYear,
                decimal value)
            {
                Value = value;
                MonthYear = monthYear;
            }

            public MonthYear MonthYear { get; }
            public decimal Value { get; }
        }

    }
}