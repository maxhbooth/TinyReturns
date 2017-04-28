namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public class MonthlyReturnDto
    {
        public int ReturnSeriesId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal ReturnValue { get; set; }
    }
}