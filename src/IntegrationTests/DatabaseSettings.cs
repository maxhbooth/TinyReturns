using System.Configuration;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;

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