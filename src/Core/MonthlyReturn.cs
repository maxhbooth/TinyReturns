using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class MonthlyReturn
    {
        public int ReturnSeriesId { get; set; }
        public decimal ReturnValue { get; set; }
        public MonthYear MonthYear { get; set; }

        // ** Equality

        protected bool Equals(MonthlyReturn other)
        {
            return ReturnSeriesId == other.ReturnSeriesId && ReturnValue == other.ReturnValue && Equals(MonthYear, other.MonthYear);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MonthlyReturn)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ReturnSeriesId;
                hashCode = (hashCode * 397) ^ ReturnValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (MonthYear != null ? MonthYear.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(MonthlyReturn left, MonthlyReturn right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonthlyReturn left, MonthlyReturn right)
        {
            return !Equals(left, right);
        }
    }
}