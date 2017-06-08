using System.Linq;
using System;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using System.Collections.Generic;
using System.Linq;
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


        [HttpGet]
        public ActionResult Index()
        {
            var previousMonth = new MonthYear(new Clock().GetCurrentDate()).AddMonths(-1);

            var selectListItems = CreateLetterSelectItems();
            
            var model = new PortfolioPerformanceIndexModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify(),
                Selected = "0",
                NetGrossList = selectListItems
            };

            return View(model);
        }

        private SelectListItem[] CreateLetterSelectItems()
        {
            var selectListItems = new SelectListItem[2];

            selectListItems[0] = new SelectListItem()
            {
                Value = "0",
                Text = "Net"
            };
            selectListItems[1] = new SelectListItem()
            {
                Value = "1",
                Text = "Gross"
            };

            return selectListItems;
        }


        [HttpPost]
        public ActionResult Index(PortfolioPerformanceIndexModel pastMonths)
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
        public ActionResult TestIndex(MonthYear currentMonth)
            return View(model);
            //return View(_publicWebReportFacade.GetPortfolioPerforance());
        }
        [HttpPost]
        public ActionResult Index(
            PortfolioPerformanceNetGrossModel model)
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
            var selectListItems = CreateLetterSelectItems();
            string select;
            PublicWebReportFacade.PortfolioModel[] portfolioPerforance;
            if (model.Selected == "0")
            {
                portfolioPerforance = _publicWebReportFacade.GetPortfolioPerformance();
                select = "0";
            }
            else if (model.Selected == "1")
            {
                portfolioPerforance = _publicWebReportFacade.GetGrossPortfolioPerforance();
                select = "1";
            }
            else
            {
                throw new InvalidOperationException();
            }
            var resultModel = new PortfolioPerformanceIndexModel()
            {
                Portfolios = portfolioPerforance,
                NetGrossList = selectListItems,
                Selected = select
            };
            return View(resultModel);
        }
    }
}