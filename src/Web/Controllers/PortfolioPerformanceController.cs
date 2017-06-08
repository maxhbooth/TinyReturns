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
        private readonly IClock _clock;

        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            _clock = new Clock();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade, IClock clock)
        {
            _publicWebReportFacade = publicWebReportFacade;
            _clock = clock;
        }


        [HttpPost]
        public ActionResult Index(PastMonthsModel pastMonths)
        {
            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);

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
            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);

            var pastMonthsModel = new PastMonthsModel
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify()
            };
            return View(pastMonthsModel);
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