namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public class MonthlyReturnDto
    {
        public int ReturnSeriesId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal ReturnValue { get; set; }
    }
}