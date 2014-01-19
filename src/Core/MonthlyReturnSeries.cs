using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class MonthlyReturnSeries
    {
        private readonly List<MonthlyReturn> _monthlyReturns;
        private readonly FinancialMath _financialMath;

        public MonthlyReturnSeries()
        {
            _monthlyReturns = new List<MonthlyReturn>();
            _financialMath = new FinancialMath();
        }

        public int ReturnSeriesId { get; set; }
        public FeeType FeeType { get; set; }

        public void AddReturns(
            IEnumerable<MonthlyReturn> r)
        {
            _monthlyReturns.AddRange(r);
        }

        public void AddReturn(
            MonthlyReturn r)
        {
            _monthlyReturns.Add(r);
        }

        public void AddReturn(
            MonthYear monthYear,
            decimal returnValue)
        {
            _monthlyReturns.Add(new MonthlyReturn()
            {
                ReturnSeriesId = ReturnSeriesId,
                MonthYear = monthYear,
                ReturnValue = returnValue
            });
        }

        public MonthlyReturn[] GetAllMonthlyReturns()
        {
            return _monthlyReturns.ToArray();
        }

        public int MonthlyReturnsCount
        {
            get { return _monthlyReturns.Count; }
        }

        public ReturnResult CalculateReturn(
            MonthYear endMonth,
            int numberOfMonths,
            AnnualizeActionEnum annualizeAction = AnnualizeActionEnum.Annualize)
        {
            var monthRange = MonthYearRange.CreateForEndMonthAndMonthsBack(endMonth, numberOfMonths);

            var returnsInRange = _monthlyReturns.GetMonthsInRange(monthRange).ToArray();

            var result = new ReturnResult();

            if (returnsInRange.Any())
            {
                if (!returnsInRange.HasExactlyOneReturnPerMonth(monthRange))
                {
                    result.SetError("Could not find a complete / unique set of months.");
                    return result;
                }

                var linkingResult = returnsInRange.PerformGeometricLiking();

                result.SetValue(linkingResult.Value, linkingResult.Calculation);

                if (MonthsIsMoreThanYearAndAnnualizeActionSet(numberOfMonths, annualizeAction))
                {
                   var annualizedResult = _financialMath.AnnualizeByMonth(linkingResult.Value, numberOfMonths);

                    result.AppendCalculation(annualizedResult.Value, annualizedResult.Calculation);
                }
            }
            else
                result.SetError("Could not find return(s) for month(s).");

            return result;
        }

        private bool MonthsIsMoreThanYearAndAnnualizeActionSet(
            int numberOfMonths,
            AnnualizeActionEnum annualizeAction)
        {
            return (numberOfMonths > 12) && (annualizeAction == AnnualizeActionEnum.Annualize);
        }

        // ** Equality

        protected bool Equals(MonthlyReturnSeries other)
        {
            if (!_monthlyReturns.SequenceEqual(other._monthlyReturns))
                return false;

            return ReturnSeriesId == other.ReturnSeriesId && Equals(FeeType, other.FeeType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((MonthlyReturnSeries)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ReturnSeriesId * 397) ^ (FeeType != null ? FeeType.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MonthlyReturnSeries left, MonthlyReturnSeries right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonthlyReturnSeries left, MonthlyReturnSeries right)
        {
            return !Equals(left, right);
        }
    }
}