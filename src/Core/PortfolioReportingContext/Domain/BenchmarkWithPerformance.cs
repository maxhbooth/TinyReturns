using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class BenchmarkWithPerformance
    {
        private readonly ReturnSeries _returnSeries;

        public BenchmarkWithPerformance(
            int number,
            string name,
            ReturnSeries returnSeries)
        {
            _returnSeries = returnSeries;
            Name = name;
            Number = number;
        }

        public int Number { get; private set; }

        public string Name { get; private set; }

        private bool HasReturnSeries
        {
            get { return _returnSeries != null; }
        }

        public decimal? GetNetMonthlyReturn(
            MonthYear monthYear)
        {
            if (HasReturnSeries)
                return _returnSeries.GetMonthlyReturn(monthYear);

            return null;
        }

        public decimal? CalculateStandardDeviation(MonthYear inceptionMonth)
        {
            if (_returnSeries == null)
                return null;

            return _returnSeries.CalculateStandardDeviation(inceptionMonth);
        }

        public decimal? CalculateMean(MonthYear inceptionMonth)
        {
            if (_returnSeries == null)
                return null;
            return _returnSeries.CalculateMean(inceptionMonth);
        }

        public decimal? CalculateReturnAsDecimal(
            CalculateReturnRequest request)
        {
            if (HasReturnSeries)
                return _returnSeries.CalculateReturnAsDecimal(request);

            return null;
        }
    }
}