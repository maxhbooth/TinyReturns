using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;

        public ReportController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
        }

        // Used for tests
        public ReportController(
            PublicWebReportFacade publicWebReportFacade)
        {
            _publicWebReportFacade = publicWebReportFacade;
        }

        public ActionResult Index()
        {
            var monthYear = new MonthYear(2000, 1);

            return View(_publicWebReportFacade.GetPortfolioPerforance(monthYear));
        }
    }
}