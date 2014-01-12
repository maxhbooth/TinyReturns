using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class MonthlyReturnSeries
    {
        private readonly List<MonthlyReturn> _monthlyReturns;

        public MonthlyReturnSeries()
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
            MonthYear monthYear)
        {
            var monthlyReturn = _monthlyReturns.FirstOrDefault(r => r.MonthYear == monthYear);

            var result = new ReturnResult();

            if (monthlyReturn == null)
                result.SetAsError();
            else
                result.SetValue(monthlyReturn.ReturnValue);

            return result;
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