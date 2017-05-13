using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.ExcelRendering;
using Dimensional.TinyReturns.FileIo;
using Dimensional.TinyReturns.Logging;

namespace Dimensional.TinyReturns.DependencyManagement
{
    public static class DependencyManager
    {
        public static void BootstrapForTests(
            ISystemLog systemLog,
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings)
        {
            BootstrapAll(systemLog, tinyReturnsDatabaseSettings);
        }

        public static void BootstrapForSystem(
            string logName,
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings)
        {
            LoggingBootstrapper.StartupLog(logName);

            var logForNetSystemLog = new LogForNetSystemLog();

            BootstrapAll(logForNetSystemLog, tinyReturnsDatabaseSettings);
        }

        private static void BootstrapAll(
            ISystemLog systemLog,
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings)
        {
            var flatFileIo = new FlatFileIo();
            var citiReturnsFileReader = new CitiReturnsFileReader(systemLog);
            var performanceReportExcelReportView = new PerformanceReportExcelReportView();

            MasterFactory.SystemLog = systemLog;
            MasterFactory.TinyReturnsDatabaseSettings = tinyReturnsDatabaseSettings;
            MasterFactory.CitiReturnsFileReader = citiReturnsFileReader;
            MasterFactory.FlatFileIo = flatFileIo;
            MasterFactory.PerformanceReportExcelReportView = performanceReportExcelReportView;
            MasterFactory.ReturnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.PortfolioToReturnSeriesDataTableGateway = new PortfolioToReturnSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.MonthlyReturnDataTableGateway = new MonthlyReturnDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.PortfolioDataTableGateway = new PortfolioDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.BenchmarkDataTableGateway = new BenchmarkDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.BenchmarkToReturnSeriesDataTableGateway = new BenchmarkToReturnSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.PortfolioToBenchmarkDataTableGateway = new PortfolioToBenchmarkDataTableGateway(tinyReturnsDatabaseSettings, systemLog);

        }
    }
}