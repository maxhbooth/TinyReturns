using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
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
            MasterFactory.ReturnsSeriesDataTableGateway = new ReturnsSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.MonthlyReturnsDataTableGateway = new MonthlyReturnsDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.CitiReturnsFileReader = citiReturnsFileReader;
            MasterFactory.InvestmentVehicleDataTableGateway = new InvestmentVehicleDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.FlatFileIo = flatFileIo;
            MasterFactory.PerformanceReportExcelReportView = performanceReportExcelReportView;
            MasterFactory.ReturnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.PortfolioToReturnSeriesDataTableGateway = new PortfolioToReturnSeriesDataTableGateway(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.MonthlyReturnDataTableGateway = new MonthlyReturnDataTableGateway(tinyReturnsDatabaseSettings, systemLog);

        }
    }
}