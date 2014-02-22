using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public static class MonthlyReturnExtensions
    {
        public static IEnumerable<MonthlyReturn> GetMonthsInRange(
            this IEnumerable<MonthlyReturn> allReturns,
            MonthYearRange monthYearRange)
        {
            return allReturns.Where(r => monthYearRange.IsMonthInRange(r.MonthYear));
        }

        public static MonthlyReturn GetMonth(
            this IEnumerable<MonthlyReturn> allReturns,
            MonthYear monthYear)
        {
            return allReturns.FirstOrDefault(r => r.MonthYear == monthYear);
        }

        public static decimal[] GetReturnValues(
            this IEnumerable<MonthlyReturn> allReturns)
        {
            return allReturns.Select(r => r.ReturnValue).ToArray();
        }

        public static FinancialMathResult PerformGeometricLiking(
            this IEnumerable<MonthlyReturn> allReturns)
        {
            var financialMath = new FinancialMath();

            var returnValues = allReturns.GetReturnValues();

            return financialMath.PerformGeometricLinking(returnValues);
        }

        public static bool WeDoNotHaveExactlyOneReturnPerMonth(
            this IEnumerable<MonthlyReturn> allReturns,
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