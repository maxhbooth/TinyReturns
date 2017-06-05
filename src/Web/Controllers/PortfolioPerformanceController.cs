using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using System.Collections.Generic;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioPerformanceController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;

        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade)
        {
            _publicWebReportFacade = publicWebReportFacade;
        }

        public ActionResult Index()
        {
            return View(_publicWebReportFacade.GetPortfolioPerforance());
        }

        public ActionResult LoadNetGross()
        {
            List<SelectListItem> netgrossList = new List<SelectListItem>();
            netgrossList.Add(new SelectListItem
            {
                Selected = true,
                Text = "Net",
                Value = "Net"
            });
            netgrossList.Add(new SelectListItem
            {
                Text = "Gross",
                Value = "Gross"
            });
            ViewData["netgross"] = netgrossList;
            return View();
        }

        private SelectListItem[] CreateLetterSelectItems()
        {
            var selectListItems = new SelectListItem[27];

            selectListItems[0] = new SelectListItem()
            {
                Value = "0",
                Text = "Net"
            };

            return selectListItems;
        }
    }
}