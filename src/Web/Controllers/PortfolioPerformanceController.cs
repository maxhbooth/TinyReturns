using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Web.Models;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioPerformanceController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;
        private readonly PastMonthsModel pastMonthsModel;
        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            pastMonthsModel = WebHelper.GetAllPossibleMonths();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade)
        {
            _publicWebReportFacade = publicWebReportFacade;
            pastMonthsModel = WebHelper.GetAllPossibleMonths();
        }


        [HttpPost]
                public ActionResult Index(PastMonthsModel pastMonths)
                {
                    if (pastMonths != null)
                    {
                        var monthYearArray = pastMonths.MonthYear.Split('/');
                        var monthYear = new MonthYear(int.Parse(monthYearArray[1]), int.Parse(monthYearArray[0]));

                        pastMonthsModel.Portfolios = _publicWebReportFacade.GetPortfolioPerformance(monthYear);
                    }
                    else
                    {
                        pastMonthsModel.Portfolios = _publicWebReportFacade.GetPortfolioPerformance();
                    }
                    return View(pastMonthsModel);
                }

        [HttpGet]
        public ActionResult Index()
        {

            pastMonthsModel.Portfolios = _publicWebReportFacade.GetPortfolioPerformance();

            return View(pastMonthsModel);
        }
    }

    public static class WebHelper
    {
        public static PastMonthsModel GetAllPossibleMonths()
        {
            var allMonthlyReturns = MasterFactory.MonthlyReturnDataTableGateway.GetAll();

            var minMonthlyReturnYear = allMonthlyReturns.Min(monthlyReturn => monthlyReturn.Year);

            var minMonthlyReturnMonth = allMonthlyReturns.Where(m => m.Year == minMonthlyReturnYear)
                .Min(monthlyReturn => monthlyReturn.Month);

            var maxMonthlyReturnYear = allMonthlyReturns.Max(monthlyReturn => monthlyReturn.Year);

            var maxMonthlyReturnMonth = allMonthlyReturns.Where(m => m.Year == minMonthlyReturnYear)
                .Max(monthlyReturn => monthlyReturn.Month);

            var allPossibleMonths = new List<MonthYear>();

            for (var year = maxMonthlyReturnYear; year >= minMonthlyReturnYear; year--)
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
            var pastMonthsModel = new PastMonthsModel
            {
                MonthYears = allPossibleMonths.Select(x => new SelectListItem()
                {
                    Text = x.Month.ToString() + "/" + x.Year.ToString(),
                    Value = x.Month.ToString() + "/" + x.Year.ToString(),
                }).ToArray()
            };

            return pastMonthsModel;
        }
    }
}