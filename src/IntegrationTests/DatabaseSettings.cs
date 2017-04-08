using System.Configuration;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class DatabaseSettings : ITinyReturnsDatabaseSettings
    {
        public string TinyReturnsDatabaseConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TinyReturnsDatabase"].ConnectionString; }
        }
    }
}