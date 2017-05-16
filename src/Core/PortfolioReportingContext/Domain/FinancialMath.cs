using System;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class FinancialMath
    {
        public FinancialMathResult PerformGeometricLinking(decimal[] values)
        {
            var returnValue = values[0];
            var calculation = values[0].ToString();

            for (int i = 1; i < values.Length; ++i)
            {
                returnValue = (1 + returnValue) * (1 + values[i]) - 1;
                calculation = string.Format("((1 + {0}) * (1 + {1}) - 1)", calculation, values[i]);
            }

            var result = new FinancialMathResult(
                returnValue,
                calculation + " = " + returnValue);

            return result;
        }

        public FinancialMathResult AnnualizeByMonth(
            decimal value,
            int monthCount)
        {
            const int monthsPerYear = 12;

            var valueAsDouble = Convert.ToDouble(value);

            var baseVal = 1 + valueAsDouble;
            var baseValCalc = string.Format("(1 + {0})", value);

            var exponentVal = monthsPerYear * 1.0 / monthCount;
            var exponentValCalc = string.Format("({0} * 1 / {1})", monthsPerYear, monthCount);

            var pow = Math.Pow(baseVal, exponentVal) - 1;
            var calculation = string.Format("({0} ^ {1}) - 1", baseValCalc, exponentValCalc);

            var result = new FinancialMathResult();
            result.Value = Convert.ToDecimal(pow);
            result.Calculation = calculation + " = " + Convert.ToDecimal(pow);

            return result;
        }

    }

    public class FinancialMathResult
    {
        public FinancialMathResult()
        {
        }

        public FinancialMathResult(decimal value, string calculation)
        {
            Value = value;
            Calculation = calculation;
        }

        public decimal Value { get; set; }
        public string Calculation { get; set; }
    }

}