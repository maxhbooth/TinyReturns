namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public class PortfolioToReturnSeriesDto
    {
        public const char NetSeriesTypeCode = 'N';
        public const char GrossSeriesTypeCode = 'G';

        public int PortfolioNumber { get; set; }
        public int ReturnSeriesId { get; set; }
        public char SeriesTypeCode { get; set; }
    }
}