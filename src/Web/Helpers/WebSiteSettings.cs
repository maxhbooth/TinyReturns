using System.Configuration;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Web.Helpers
{
    public class WebSiteSettings : ITinyReturnsDatabaseSettings
    {
        public string TinyReturnsDatabaseConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TinyReturnsDatabase"].ConnectionString; }
        }
    }
}