using System.Linq;

namespace Dimensional.TinyReturns.Core.PerformanceReport
{
    public class PerformanceReportExcelReportModel
    {
        public string MonthText { get; set; }
        public PerformanceReportExcelReportRecordModel[] Records { get; set; }

        public PerformanceReportExcelReportRecordModel[] GetPortfolioFeeRecords(
            string portfolioName,
            string feeType)
        {
            return Records
                .Where(r => r.Name == portfolioName && r.FeeType == feeType)
                .ToArray();
        }
    }

    public class PerformanceReportExcelReportRecordModel
    {
        public int EntityNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FeeType { get; set; }
        public decimal? OneMonth { get; set; }
        public decimal? ThreeMonths { get; set; }
        public decimal? TwelveMonths { get; set; }
        public decimal? YearToDate { get; set; }
    }

}