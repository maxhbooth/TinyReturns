using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

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
            return View(_publicWebReportFacade.GetPortfolioPerformance());
        }


        public ActionResult PastMonths()
        {
            var allMonthlyReturns = MasterFactory.MonthlyReturnDataTableGateway.GetAll();

            var minMonthlyReturnYear = allMonthlyReturns.Min(monthlyReturn=> monthlyReturn.Year);

            var minMonthlyReturnMonth = allMonthlyReturns.Where(m => m.Year == minMonthlyReturnYear)
                .Min(monthlyReturn => monthlyReturn.Month);

            var maxMonthlyReturnYear = allMonthlyReturns.Max(monthlyReturn => monthlyReturn.Year);

            var maxMonthlyReturnMonth = allMonthlyReturns.Where(m => m.Year == minMonthlyReturnYear)
                .Max(monthlyReturn => monthlyReturn.Month);

            var maxMonthYear = new MonthYear(maxMonthlyReturnYear, maxMonthlyReturnMonth);

            var monthsBack = 12 * (maxMonthlyReturnYear - minMonthlyReturnYear) +
                             (maxMonthlyReturnMonth - minMonthlyReturnMonth);

            var allPossibleMonths =new List<MonthYear>();

            for (var year = maxMonthlyReturnYear ; year>=minMonthlyReturnYear;year--)
                {
                    for (var month = 12; month >= 1; month--)
                    {
                        if (year == maxMonthlyReturnYear && month >= maxMonthlyReturnMonth)
                        {
                            //do nothing
                        }
                        else if (year == minMonthlyReturnYear && month <= minMonthlyReturnMonth)
                        {
                            //do nothing
                        }
                        else
                        {
                            allPossibleMonths.Add(new MonthYear(year, month));
                        }
                    }
                }
            var pastMonthsModel = new PastMonthsModel();
            pastMonthsModel.MonthYears = allPossibleMonths.Select(x => new SelectListItem()
            {
                Text = x.Month.ToString() + "/" + x.Year.ToString(),
                Value = x.Month.ToString() + "/" + x.Year.ToString(),
            }).ToArray();

            return View(pastMonthsModel);
        }

        public ActionResult PastMonthsGraph()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PastMonthsGraph(PastMonthsModel pastMonthsModel)
        {

            var MonthYearArray =  pastMonthsModel.MonthYear.Split('/');
            var monthYear = new MonthYear(int.Parse(MonthYearArray[1]), int.Parse(MonthYearArray[0]));

            return View(_publicWebReportFacade.GetPortfolioPerformance(monthYear));

        }
    }

    public class PastMonthsModel
    {
        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
    }

    public class TinyReturnsModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios;
        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
    }
}