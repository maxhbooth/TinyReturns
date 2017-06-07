using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Web.Models;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioPerformanceController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;
        private readonly SelectListItem[] _allMonths;
        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            _allMonths = WebHelper.GetAllPossibleMonths();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade, MonthlyReturnDataTableGateway monthlyReturnDataTableGateway)
        {
            _publicWebReportFacade = publicWebReportFacade;
            _allMonths = WebHelper.GetAllPossibleMonths(monthlyReturnDataTableGateway);
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
                            MonthYears = _allMonths,
                            MonthYear = monthYear.Stringify()
                        };
                    }
                    else
                    {
                        pastMonthsModel = new PastMonthsModel()
                        {
                            Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                            MonthYears = _allMonths,
                            MonthYear = _publicWebReportFacade.monthBeingReportedOn.Stringify()
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
                MonthYears = _allMonths
            };
            if (_publicWebReportFacade.monthBeingReportedOn != null)
            {
                pastMonthsModel.MonthYear = _publicWebReportFacade.monthBeingReportedOn.Stringify();
            }
            return View(pastMonthsModel);
        }
    }

    public static class WebHelper
    {
        public static SelectListItem[] GetAllPossibleMonths()
        {
            var allMonthlyReturns = MasterFactory.MonthlyReturnDataTableGateway.GetAll();

            if (allMonthlyReturns.Length == 0)
            {

               return new SelectListItem[0];

            }

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

            var monthYears = allPossibleMonths.Select(x => new SelectListItem()
            {
                Text = x.Stringify(),
                Value = x.Stringify(),
            }).ToArray();

            return monthYears;
        }

        //for testing

        public static SelectListItem[] GetAllPossibleMonths(MonthlyReturnDataTableGateway monthlyReturnDataTableGateway)
        {
            var allMonthlyReturns = monthlyReturnDataTableGateway.GetAll();

            if (allMonthlyReturns.Length == 0)
            {
                return new SelectListItem[0];
            }

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
                    if (year == maxMonthlyReturnYear && month > maxMonthlyReturnMonth)
                    {
                        //do nothing
                    }
                    else if (year == minMonthlyReturnYear && month < minMonthlyReturnMonth)
                    {
                        //do nothing
                    }
                    else
                    {
                        allPossibleMonths.Add(new MonthYear(year, month));
                    }
                }
            }

            var monthYears = allPossibleMonths.Select(x => new SelectListItem()
            {
                Text = x.Stringify(),
                Value = x.Stringify(),
            }).ToArray();


            return monthYears;
        }

    }
}