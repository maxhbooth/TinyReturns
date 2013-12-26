namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        private static ISystemLog _systemLog;
        private static ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;
        private static IReturnsSeriesRepository _returnsSeriesRepository;

        public static ISystemLog SystemLog
        {
            set { _systemLog = value; }
        }

        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings
        {
            set { _tinyReturnsDatabaseSettings = value; }
        }

        public static IReturnsSeriesRepository ReturnsSeriesRepository
        {
            set { _returnsSeriesRepository = value; }
        }

        public static ISystemLog GetSystemLog()
        {
            return _systemLog;
        }

        public static ITinyReturnsDatabaseSettings GetTinyReturnsDatabaseSettings()
        {
            return _tinyReturnsDatabaseSettings;
        }

        public static IReturnsSeriesRepository GetReturnsSeriesRepository()
        {
            return _returnsSeriesRepository;
        }
    }
}