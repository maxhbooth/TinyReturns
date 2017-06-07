﻿namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport
{
    public class PerformanceReportExcelReportModel
    {
        public string MonthText { get; set; }
        public PerformanceReportExcelReportRecordModel[] Records { get; set; }
    }

    public class PerformanceReportExcelReportRecordModel
    {
        public int EntityNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FeeType { get; set; }
        public decimal? OneMonth { get; set; }
        public decimal? ThreeMonths { get; set; }
        public decimal? SixMonths { get; set; }
        public decimal? TwelveMonths { get; set; }
        public decimal? YearToDate { get; set; }
        public decimal? QuarterToDate { get; set; }
        public decimal? FirstFullMonth { get; set; }
        public decimal? StandardDeviation { get; set; }
        public decimal? Mean { get; set; }
    }

}