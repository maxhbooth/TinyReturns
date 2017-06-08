using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Web.Models;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioPerformanceController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;
        private readonly MonthYear _currentMonth;

        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            _currentMonth = new MonthYear(new Clock().GetCurrentDate());
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade, MonthYear currentMonth)
        {
            _publicWebReportFacade = publicWebReportFacade;
            _currentMonth = currentMonth;
        }


        [HttpPost]
                public ActionResult Index(PastMonthsModel pastMonths)
        {
            PastMonthsModel pastMonthsModel;
                    if (pastMonths != null)
                    {
                        var monthYearArray = pastMonths.MonthYear.Split('/');
                        var monthYear = new MonthYear(int.Parse(monthYearArray[1]), int.Parse(monthYearArray[0]));

                        pastMonthsModel = new PastMonthsModel()
                        {
                            Portfolios = _publicWebReportFacade.GetPortfolioPerformance(monthYear),
                            MonthYears = WebHelper.GetDesiredMonths(_currentMonth),
                            MonthYear = monthYear.Stringify()
                        };
                    }
                    else
                    {
                        pastMonthsModel = new PastMonthsModel()
                        {
                            Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                            MonthYears = WebHelper.GetDesiredMonths(_currentMonth),
                            MonthYear = _currentMonth.Stringify()
                        };
                    }
                    return View(pastMonthsModel);
                }

        [HttpGet]
        public ActionResult Index()
        {
            var pastMonthsModel = new PastMonthsModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(_currentMonth),
                MonthYear = _currentMonth.Stringify()
            };
            return View(pastMonthsModel);
        }
    }

    public static class WebHelper
    {
        public static SelectListItem[] GetDesiredMonths(MonthYear currentMonth)
        {

            var monthsInRange = MonthYearRange.CreateForEndMonthAndMonthsBack(currentMonth, 36).GetMonthsInRange();

            var monthYears = monthsInRange.Select(x => new SelectListItem()
            {
                Text = x.Stringify(),
                Value = x.Stringify(),
            }).ToArray();

            return monthYears;
        }
    }
}