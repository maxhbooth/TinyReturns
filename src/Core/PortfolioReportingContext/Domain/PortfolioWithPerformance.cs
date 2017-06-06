using System;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class PortfolioWithPerformance
    {
        private readonly ReturnSeries _netReturnSeries;
        private readonly ReturnSeries _grossReturnSeries;
        private BenchmarkWithPerformance[] _benchmarkWithPerformances;

        public PortfolioWithPerformance(
            int number,
            string name,
            ReturnSeries netReturnSeries,
            ReturnSeries grossReturnSeries,
            BenchmarkWithPerformance[] benchmarks)
        {
            _benchmarkWithPerformances = benchmarks;
            _grossReturnSeries = grossReturnSeries;
            _netReturnSeries = netReturnSeries;
            Name = name;
            Number = number;
        }

        public int Number { get; private set; }
        public string Name { get; private set; }

        public decimal? GetNetMonthlyReturn(
            MonthYear monthYear)
        {
            if (_netReturnSeries == null)
                return null;

            return _netReturnSeries.GetMonthlyReturn(monthYear);
        }

        public decimal? GetNetMonthlyReturnPercent(
            MonthYear monthYear)
        {
            if (_netReturnSeries == null)
                return null;
            //this is wrong
            decimal? result = _netReturnSeries.GetMonthlyReturnPercent(monthYear);
            return result;
            //return _netReturnSeries.GetMonthlyReturn(monthYear);
        }

        public decimal? CalculateNetReturnAsDecimal(
            CalculateReturnRequest request)
        {
            if (_netReturnSeries == null)
                return null;

            return _netReturnSeries.CalculateReturnAsDecimal(request);
        }


        public decimal? CalculateNetReturnAsPercent(CalculateReturnRequest request)
        {
            
            if (_netReturnSeries == null)
                return null;
            decimal? result = _netReturnSeries.CalculateReturnAsDecimal(request);
            if (result.HasValue) {
                result = (result * 100);
                //return decimal.Round((decimal)result, 2, MidpointRounding.AwayFromZero);
                return result;
            }
            return null;
            //throw new NotImplementedException();
        }


        public ReturnResult CalculateNetReturn(
            CalculateReturnRequest request)
        {
            if (_netReturnSeries == null)
                return ReturnResult.CreateWithError("Portfolio has not net return series.");

            return _netReturnSeries.CalculateReturn(request);
        }

        public ReturnResult CalculateGrossReturn(
            CalculateReturnRequest request)
        {
            if (_grossReturnSeries == null)
                return ReturnResult.CreateWithError("Portfolio has not gross return series.");

            return _grossReturnSeries.CalculateReturn(request);
        }

        public decimal? CalculateNetStandardDeviation()
        {
            if (_netReturnSeries == null)
                return null;

            return _netReturnSeries.CalculateStandardDeviation(CalculateNetMean());
        }

        public decimal? CalculateNetMean()
        {
            if (_netReturnSeries == null)
                return null;
            return _netReturnSeries.CalculateMean();
        }

        public decimal? CalculateGrossStandardDeviation()
        {
            if (_grossReturnSeries == null)
                return null;

            return _grossReturnSeries.CalculateStandardDeviation(CalculateGrossMean());
        }

        public decimal? CalculateGrossMean()
        {
            if (_grossReturnSeries == null)
                return null;
            return _grossReturnSeries.CalculateMean();
        }

        public bool HasNetReturnSeries
        {
            get { return _netReturnSeries != null; }
        }

        public bool HasGrossReturnSeries
        {
            get { return _grossReturnSeries != null; }
        }

        public ReturnSeries.MonthlyReturn[] GetNetReturnsInRange(
            MonthYearRange monthYearRange)
        {
            if (HasNetReturnSeries)
                return _netReturnSeries.GetMonthsInRange(monthYearRange);

            return new ReturnSeries.MonthlyReturn[0];
        }

        public ReturnSeries.MonthlyReturn[] GetGrossReturnsInRange(
            MonthYearRange monthYearRange)
        {
            if (HasGrossReturnSeries)
                return _grossReturnSeries.GetMonthsInRange(monthYearRange);

            return new ReturnSeries.MonthlyReturn[0];
        }

        public BenchmarkWithPerformance[] GetAllBenchmarks()
        {
            return _benchmarkWithPerformances
                .OrderBy(b => b.Number)
                .ToArray();
        }
    }
}