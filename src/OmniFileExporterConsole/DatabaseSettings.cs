using System.Configuration;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.OmniFileExporterConsole
{
    public class DatabaseSettings : ITinyReturnsDatabaseSettings
    {
        public string TinyReturnsDatabaseConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TinyReturnsDatabase"].ConnectionString; }
        }
    }
}