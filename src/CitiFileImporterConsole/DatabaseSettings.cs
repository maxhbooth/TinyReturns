using System.Configuration;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.CitiFileImporterConsole
{
    public class DatabaseSettings : ITinyReturnsDatabaseSettings
    {
        public string TinyReturnsDatabaseConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TinyReturnsDatabase"].ConnectionString; }
        }
    }
}