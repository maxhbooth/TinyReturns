using System.IO;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.ReportGeneratorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("OmniFileExporterConsole", new DatabaseSettings());

            var performanceReportExcelReportCreator = MasterFactory.GetPerformanceReportExcelReportCreator();

            const string excelFilePath = "c:\\temp\\ExcelReport.xlsx";

            File.Delete(excelFilePath);

            performanceReportExcelReportCreator.CreateReport(new MonthYear(2013, 6), excelFilePath);
        }
    }
}
