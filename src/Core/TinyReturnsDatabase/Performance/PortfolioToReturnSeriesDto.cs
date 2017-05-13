using System.Linq;

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

    public static class PortfolioToReturnSeriesDtoExtensions
    {
        public static PortfolioToReturnSeriesDto Find(
            this PortfolioToReturnSeriesDto[] dtos,
            int portfolioNumber,
            char seriesType)
        {
            return dtos.FirstOrDefault(d =>
                    d.PortfolioNumber == portfolioNumber &&
                    d.SeriesTypeCode == seriesType);
        }

        public static PortfolioToReturnSeriesDto FindNet(
            this PortfolioToReturnSeriesDto[] dtos,
            int portfolioNumber)
        {
            return Find(
                dtos,
                portfolioNumber,
                PortfolioToReturnSeriesDto.NetSeriesTypeCode);
        }

        public static PortfolioToReturnSeriesDto FindGross(
            this PortfolioToReturnSeriesDto[] dtos,
            int portfolioNumber)
        {
            return Find(
                dtos,
                portfolioNumber,
                PortfolioToReturnSeriesDto.GrossSeriesTypeCode);
        }
    }
}