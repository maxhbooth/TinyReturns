﻿using System.Linq;
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
        public ActionResult Index(
            PortfolioPerformanceIndexModel model)
        {
            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);

            var selectListItems = CreateLetterSelectItems();
            string select;

            PublicWebReportFacade.PortfolioModel[] portfolioPerforance;
            if (model.Selected == "0")
            {
                portfolioPerforance = _publicWebReportFacade.GetPortfolioPerformance(previousMonth);
                select = "0";
            }
            else if (model.Selected == "1")
            {
                portfolioPerforance = _publicWebReportFacade.GetGrossPortfolioPerforance(previousMonth);
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
                Selected = select,
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify()
            };

            return View(resultModel);
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