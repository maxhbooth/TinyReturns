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

#if DEBUG
            string excelFilePath = "c:\\temp\\ExcelReport.xlsx";
            var monthYear = new MonthYear(2016, 6);
#else
            string excelFilePath = args[0];

            var year = int.Parse(args[1]);
            var month = int.Parse(args[2]);
            var monthYear = new MonthYear(year, month);
#endif

            File.Delete(excelFilePath);
            
            performanceReportExcelReportCreator.CreateReport(monthYear, excelFilePath);
        }
    }
}
