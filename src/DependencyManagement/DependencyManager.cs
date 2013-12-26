using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Database;

namespace Dimensional.TinyReturns.DependencyManagement
{
    public static class DependencyManager
    {
        public static void Bootstrap(
            ISystemLog systemLog,
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings)
        {
            MasterFactory.SystemLog = systemLog;
            MasterFactory.TinyReturnsDatabaseSettings = tinyReturnsDatabaseSettings;
            MasterFactory.ReturnsSeriesRepository = new TinyReturnsDatabase(tinyReturnsDatabaseSettings, systemLog);
        }
    }
}