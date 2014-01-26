using System.Web.Http;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PublicWebSite;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PublicWebController : ApiController
    {
        private readonly PortfolioListPageAdapter _portfolioListPageAdapter;

        public PublicWebController()
        {
            _portfolioListPageAdapter = MasterFactory.GetPortfolioListPageAdapter();
        }

        public PublicWebController(
            PortfolioListPageAdapter portfolioListPageAdapter)
        {
            _portfolioListPageAdapter = portfolioListPageAdapter;
        }

        public PortfolioListRecord[] Get(string id)
        {
            var split = id.Split('-');

            var year = int.Parse(split[0]);
            var month = int.Parse(split[1]);

            var monthYear = new MonthYear(year, month);

            return _portfolioListPageAdapter.GetPortfolioPageRecords(monthYear);
        }
    }
}