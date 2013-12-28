using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        private static ISystemLog _systemLog;
        public static ISystemLog SystemLog { set { _systemLog = value; } }
        public static ISystemLog GetSystemLog() { return _systemLog; }

        private static ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { set { _tinyReturnsDatabaseSettings = value; } }
        public static ITinyReturnsDatabaseSettings GetTinyReturnsDatabaseSettings() { return _tinyReturnsDatabaseSettings; }

        private static IReturnsSeriesDataRepository _returnsSeriesDataRepository;
        public static IReturnsSeriesDataRepository ReturnsSeriesDataRepository { set { _returnsSeriesDataRepository = value; } }
        public static IReturnsSeriesDataRepository GetReturnsSeriesRepository() { return _returnsSeriesDataRepository; }

        private static ICitiReturnsFileReader _citiReturnsFileReader;
        public static ICitiReturnsFileReader CitiReturnsFileReader { set { _citiReturnsFileReader = value; } }
        public static ICitiReturnsFileReader GetCitiReturnsFileReader() { return _citiReturnsFileReader; }

        public static CitiReturnSeriesImporter GetCitiReturnSeriesImporter()
        {
            return new CitiReturnSeriesImporter(
                _returnsSeriesDataRepository,
                _citiReturnsFileReader);
        }
    }
}