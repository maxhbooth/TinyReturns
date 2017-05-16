namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport
{
    public interface IPerformanceReportExcelReportView
    {
        void RenderReport(
            PerformanceReportExcelReportModel model,
            string fullFilePath);
    }
}