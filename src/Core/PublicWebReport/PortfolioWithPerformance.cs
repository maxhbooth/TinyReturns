using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PortfolioWithPerformance
    {
        private readonly ReturnSeries _netReturnSeries;
        private readonly ReturnSeries _grossReturnSeries;

        public PortfolioWithPerformance(
            int number,
            string name,
            ReturnSeries netReturnSeries,
            ReturnSeries grossReturnSeries)
        {
            _grossReturnSeries = grossReturnSeries;
            _netReturnSeries = netReturnSeries;
            Name = name;
            Number = number;
        }

        public int Number { get; }
        public string Name { get; }

        public decimal? GetNetMonthlyReturn(
            MonthYear monthYear)
        {
            if (_netReturnSeries == null)
                return null;

            return _netReturnSeries.GetMonthlyReturn(monthYear);
        }
    }
}