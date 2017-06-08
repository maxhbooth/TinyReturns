using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
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


        [HttpPost]
        public ActionResult Index(PastMonthsModel pastMonths)
        {
            MonthYear previousMonth;
            if (pastMonths.currentMonth == null)

                previousMonth = new MonthYear(new Clock().GetCurrentDate()).AddMonths(-1);
            else
                previousMonth = pastMonths.currentMonth; //for testing

            PastMonthsModel pastMonthsModel;

            var monthYearArray = pastMonths.MonthYear.Split('/');
            var monthYear = new MonthYear(int.Parse(monthYearArray[1]), int.Parse(monthYearArray[0]));

            pastMonthsModel = new PastMonthsModel
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(monthYear.AddMonths(1)),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = monthYear.Stringify()
            };

            return View(pastMonthsModel);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var previousMonth = new MonthYear(new Clock().GetCurrentDate()).AddMonths(-1);

            var pastMonthsModel = new PastMonthsModel
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify()
            };

            return View(pastMonthsModel);
        }


        [HttpGet]
        public ActionResult TestIndex(MonthYear currentMonth)
        {
            var previousMonth = currentMonth.AddMonths(-1);

            var pastMonthsModel = new PastMonthsModel
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify(),
                currentMonth = currentMonth
            };

            return View("Index", pastMonthsModel);
        }
    }

    public static class WebHelper
    {
        public static SelectListItem[] GetDesiredMonths(MonthYear currentMonth)
        {
            var monthsInRange = MonthYearRange.CreateForEndMonthAndMonthsBack(currentMonth, 36).GetMonthsInRange();

            var monthYears = monthsInRange.Select(x => new SelectListItem
            {
                Text = x.Stringify(),
                Value = x.Stringify()
            }).ToArray();

            return monthYears;
        }
    }
}