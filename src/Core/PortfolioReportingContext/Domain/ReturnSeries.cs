using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class ReturnSeries
    {
        private readonly MonthlyReturn[] _monthlyReturns;
        private readonly FinancialMath _financialMath;

        public ReturnSeries(
            int id,
            MonthlyReturn[] monthlyReturns)
        {
            Id = id;
            _monthlyReturns = monthlyReturns;

            _financialMath = new FinancialMath();
        }

        public int Id { get; private set; }

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

            public MonthYear MonthYear { get; private set; }
            public decimal Value { get; private set; }
        }

        public decimal? CalculateReturnAsDecimal(
            CalculateReturnRequest request)
        {
            var result = CalculateReturn(request);

            return result.GetNullValueOnError();
        }

        public decimal? CalculateStandardDeviation(MonthYear inceptionMonth)
        {
            var mean = CalculateMean(inceptionMonth);

            var values = _monthlyReturns.Where(x => x.MonthYear >= inceptionMonth).Select(x => x.Value).ToArray();

            if (mean == null || values.Length < 12)
            {
                return null;
            }

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (mean.Value - values[i]) * (mean.Value - values[i]);
            }

            var standardDeviation = (Decimal) Math.Sqrt((Double) values.Sum() / values.Length);

            return standardDeviation;
        }

        public decimal? CalculateMean(MonthYear inceptionMonth)
        {
            var valuesSinceInception = _monthlyReturns.Where(x => x.MonthYear >= inceptionMonth).Select(x => x.Value).ToArray();

            if (valuesSinceInception == null || valuesSinceInception.Length == 0)
            {
                return null;
            }

            return valuesSinceInception.Sum() / valuesSinceInception.Length;
        }

        public ReturnResult CalculateReturn(
            CalculateReturnRequest request)
        {
            var monthRange = MonthYearRange.CreateForEndMonthAndMonthsBack(request.EndMonth, request.NumberOfMonths);

            var returnsInRange = GetMonthsInRange(monthRange);

            var result = new ReturnResult();

            if (!returnsInRange.Any())
                return ReturnResult.CreateWithError("Could not find return(s) for month(s).");

            if (WeDoNotHaveExactlyOneReturnPerMonth(returnsInRange, monthRange))
                return ReturnResult.CreateWithError("Could not find a complete set of months."); ;

            return PerformReturnCalculation(request, returnsInRange, result);
        }

        public MonthlyReturn[] GetMonthlyReturns()
        {
            return this._monthlyReturns;
        }

        public MonthlyReturn[] GetMonthsInRange(
            MonthYearRange monthYearRange)
        {
            return _monthlyReturns
                .Where(r => monthYearRange.IsMonthInRange(r.MonthYear))
                .ToArray();
        }

        private ReturnResult PerformReturnCalculation(
            CalculateReturnRequest request,
            MonthlyReturn[] returnsInRange,
            ReturnResult result)
        {
            var decimalValues = returnsInRange
                .Select(r => r.Value)
                .ToArray();

            var linkingResult = _financialMath.PerformGeometricLinking(decimalValues);

            result.SetValue(linkingResult.Value, linkingResult.Calculation);

            result = AnnaulizeIfNeeded(request, result);
            
            return result;
        }

        private ReturnResult AnnaulizeIfNeeded(
            CalculateReturnRequest request,
            ReturnResult result)
        {
            if (request.MonthsIsMoreThanYearAndAnnualizeActionSet())
            {
                var annualizedResult = _financialMath.AnnualizeByMonth(result.Value, request.NumberOfMonths);

                result.AppendToCalculation(annualizedResult.Value, annualizedResult.Calculation);
            }

            return result;
        }

        public static bool WeDoNotHaveExactlyOneReturnPerMonth(
            IEnumerable<MonthlyReturn> allReturns,
            MonthYearRange monthYearRange)
        {
            var exactlyOneMonth = true;

            monthYearRange.ForEachMonthInRange(
                m =>
                {
                    if (allReturns.Count(r => r.MonthYear == m) != 1)
                        exactlyOneMonth = false;
                });

            return !exactlyOneMonth;
        }

    }
}