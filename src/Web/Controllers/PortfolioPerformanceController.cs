using System;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
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

        public ActionResult Index()
        {
            var selectListItems = CreateLetterSelectItems();
            //initialize all properties of model
            var model = new PortfolioPerformanceNetGrossModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                Selected = "0",
                NetGrossList = selectListItems
            };

            return View(model);
            //return View(_publicWebReportFacade.GetPortfolioPerforance());
        }
        [HttpPost]
        public ActionResult Index(
            PortfolioPerformanceNetGrossModel model)
        {
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
            var resultModel = new PortfolioPerformanceNetGrossModel()
            {
                Portfolios = portfolioPerforance,
                NetGrossList = selectListItems,
                Selected = select
            };
            return View(resultModel);
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
    }
}