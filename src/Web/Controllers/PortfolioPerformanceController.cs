﻿using System.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Web.Models;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioPerformanceController : Controller
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;
        private readonly IClock _clock;
        private readonly ICountriesDataTableGateway _countriesDataTableGateway;

        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            _countriesDataTableGateway = MasterFactory.CountriesDataTableGateway;
            _clock = new Clock();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade, IClock clock,
            ICountriesDataTableGateway countriesDataTableGateway)
        {
            _publicWebReportFacade = publicWebReportFacade;
            _clock = clock;
            _countriesDataTableGateway = countriesDataTableGateway;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);

            var selectListItems = WebHelper.CreateLetterSelectItems();

            var model = new PortfolioPerformanceIndexModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify(),
                SelectedTypeOfReturn = "0",
                NetGrossList = selectListItems,
                Countries = WebHelper.GetAllCountries(_countriesDataTableGateway),

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(
            PortfolioPerformanceIndexModel model)
        {

            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);
            var monthYearArray = model.MonthYear.Split('/');
            var monthYear = new MonthYear(int.Parse(monthYearArray[1]), int.Parse(monthYearArray[0]));

            var selectListItems = WebHelper.CreateLetterSelectItems();

            PublicWebReportFacade.PortfolioModel[] portfolioPerformance;
            if (model.SelectedTypeOfReturn == "0")
            {
                portfolioPerformance = _publicWebReportFacade.GetPortfolioPerformance(monthYear);
            }
            else if (model.SelectedTypeOfReturn == "1")
            {
                portfolioPerformance = _publicWebReportFacade.GetGrossPortfolioPerformance(monthYear);
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (model.SelectedCountry == null || model.SelectedCountry == "Show All")
            {
                //do nothing
            }
            else
            {
                portfolioPerformance = portfolioPerformance.Where(x => x.Country == model.SelectedCountry).ToArray();
            }

            var resultModel = new PortfolioPerformanceIndexModel()
            {
                Portfolios = portfolioPerformance,
                NetGrossList = selectListItems,
                SelectedTypeOfReturn = model.SelectedTypeOfReturn,
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = monthYear.Stringify(),
                Countries = WebHelper.GetAllCountries(_countriesDataTableGateway),
                SelectedCountry = model.SelectedCountry
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

            public static String[] GetPortolioCountries(PublicWebReportFacade.PortfolioModel[] portfolios)
            {
                var countries = portfolios.Select(x => x.Country).ToArray();

                return countries;
            }
            public static SelectListItem[] CreateLetterSelectItems()
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

            public static SelectListItem[] GetAllCountries(ICountriesDataTableGateway countriesDataTableGateway)
            {
                var countries = new List<SelectListItem>();

                countries.Add(new SelectListItem()
                {
                    Value = "Show All",
                    Text = "Show All"
                });

                foreach (var country in countriesDataTableGateway.GetAll())
                {
                    countries.Add(new SelectListItem()
                    {
                        Value = country.CountryName,
                        Text = country.CountryName

                    });
                }
                return countries.ToArray();
            }
        }
    }
}