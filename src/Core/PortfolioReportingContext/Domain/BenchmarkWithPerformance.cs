﻿using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using System;

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

        public decimal? GetNetMonthlyReturnPercent(
            MonthYear monthYear)
        {
            if (HasReturnSeries)
                return _returnSeries.GetMonthlyReturnPercent(monthYear);

            return null;
        }

        public decimal? CalculateReturnAsDecimal(
            CalculateReturnRequest request)
        {
            if (HasReturnSeries)
                return _returnSeries.CalculateReturnAsDecimal(request);

            return null;
        }

        public decimal? CalculateReturnAsPercent(
            CalculateReturnRequest request)
        {
            if (HasReturnSeries)//THIS MEANS NOT NULL
            {
                decimal? orig = _returnSeries.CalculateReturnAsDecimal(request);
                if (orig != null)
                {
                    decimal original = orig.Value * 100m;
                    //return decimal.Round(original, 2, MidpointRounding.AwayFromZero);
                    return original;
                }
            }

            return null;
        }
    }
}