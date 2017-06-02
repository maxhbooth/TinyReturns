using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Web.Models;

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
            var selectListItems = CreateLetterSelectItems();

            var model = new PortfolioPerformanceIndexModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerforance(),
                Letters = selectListItems
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Index(
            PortfolioPerformanceIndexModel model)
        {
            // Stuff
            return null;
        }

        private SelectListItem[] CreateLetterSelectItems()
        {
            var selectListItems = new SelectListItem[27];

            selectListItems[0] = new SelectListItem()
            {
                Value = "0",
                Text = "Show All"
            };

            for (int i = 1; i < 27; i++)
            {
                selectListItems[i] = new SelectListItem()
                {
                    Value = GetCharsAddedToA(i - 1),
                    Text = "Letter " + GetCharsAddedToA(i - 1)
                };
            }

            return selectListItems;
        }

        private static string GetCharsAddedToA(int i)
        {
            const char letterA = 'A';

            return ((char)(letterA + i)).ToString();
        }
    }
}