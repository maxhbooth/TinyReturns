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

        private SelectListItem[] CreateLetterSelectItems()
        {
            var selectListItems = new SelectListItem[26];

            for (int i = 0; i < 26; i++)
            {
                selectListItems[i] = new SelectListItem()
                {
                    Value = GetCharsAddedToA(i),
                    Text = "Letter " + GetCharsAddedToA(i)
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