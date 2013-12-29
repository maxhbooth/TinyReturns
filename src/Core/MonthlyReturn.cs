using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class MonthlyReturn
    {
        public int ReturnSeriesId { get; set; }
        public decimal ReturnValue { get; set; }
        public MonthYear MonthYear { get; set; }
    }
}