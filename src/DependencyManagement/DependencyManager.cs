using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Database;
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
            MasterFactory.SystemLog = systemLog;
            MasterFactory.TinyReturnsDatabaseSettings = tinyReturnsDatabaseSettings;
            MasterFactory.ReturnsSeriesRepository = new TinyReturnsDatabase(tinyReturnsDatabaseSettings, systemLog);
            MasterFactory.CitiReturnsFileReader = new CitiReturnsFileReader(systemLog);
        }
    }
}