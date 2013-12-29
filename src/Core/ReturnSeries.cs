using System.Collections.Generic;

namespace Dimensional.TinyReturns.Core
{
    public class ReturnSeries
    {
        private readonly List<MonthlyReturn> _monthlyReturns;

        public ReturnSeries()
        {
            _monthlyReturns = new List<MonthlyReturn>();
        }

        public int ReturnSeriesId { get; set; }
        public FeeType FeeType { get; set; }

        public void AddReturns(
            IEnumerable<MonthlyReturn> r)
        {
            _monthlyReturns.AddRange(r);
        }

        public MonthlyReturn[] GetAllMonthlyReturns()
        {
            return _monthlyReturns.ToArray();
        }

        public int MonthlyReturnsCount
        {
            get { return _monthlyReturns.Count; }
        }
    }
}