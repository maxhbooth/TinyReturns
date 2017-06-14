using System.Linq;
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
        private IPortfolioDataTableGateway _portfolioDataTableGateWay;

        public PortfolioPerformanceController()
        {
            _publicWebReportFacade = MasterFactory.GetPublicWebReportFacade();
            _countriesDataTableGateway = MasterFactory.CountriesDataTableGateway;
            _portfolioDataTableGateWay = MasterFactory.PortfolioDataTableGateway;
            _clock = new Clock();
        }

        // Used for tests
        public PortfolioPerformanceController(
            PublicWebReportFacade publicWebReportFacade, IClock clock,
            ICountriesDataTableGateway countriesDataTableGateway, IPortfolioDataTableGateway portfolioDataTableGateway)
        {
            _publicWebReportFacade = publicWebReportFacade;
            _clock = clock;
            _countriesDataTableGateway = countriesDataTableGateway;
            _portfolioDataTableGateWay = portfolioDataTableGateway;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);

            var selectListItems = CreateLetterSelectItems();

            var model = new PortfolioPerformanceIndexModel()
            {
                Portfolios = _publicWebReportFacade.GetPortfolioPerformance(),
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = previousMonth.Stringify(),
                Selected = "0",
                NetGrossList = selectListItems,
                Countries = new WebDatabaseHelper().GetAllCountries(_countriesDataTableGateway),
                PortfolioCountries = WebHelper.GetPortolioCountries(_publicWebReportFacade.GetPortfolioPerformance())
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(
            PortfolioPerformanceIndexModel model)
        {
            //insert portfolio countries into the database
            new WebDatabaseHelper().UpdatePortfolios(_portfolioDataTableGateWay, model.Portfolios);


            var previousMonth = new MonthYear(_clock.GetCurrentDate()).AddMonths(-1);
            var monthYearArray = model.MonthYear.Split('/');
            var monthYear = new MonthYear(int.Parse(monthYearArray[1]), int.Parse(monthYearArray[0]));

            var selectListItems = CreateLetterSelectItems();

            string select;

            PublicWebReportFacade.PortfolioModel[] portfolioPerformance;
            if (model.Selected == "0")
            {
                portfolioPerformance = _publicWebReportFacade.GetPortfolioPerformance(monthYear);
                select = "0";
            }
            else if (model.Selected == "1")
            {
                portfolioPerformance = _publicWebReportFacade.GetGrossPortfolioPerforance(monthYear);
                select = "1";
            }
            else
            {
                throw new InvalidOperationException();
            }

            var resultModel = new PortfolioPerformanceIndexModel()
            {
                Portfolios = portfolioPerformance,
                NetGrossList = selectListItems,
                Selected = select,
                MonthYears = WebHelper.GetDesiredMonths(previousMonth),
                MonthYear = monthYear.Stringify(),
                Countries = new WebDatabaseHelper().GetAllCountries(_countriesDataTableGateway),
                PortfolioCountries = model.PortfolioCountries
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
        }

        public class WebDatabaseHelper
        {
            public SelectListItem[] GetAllCountries(ICountriesDataTableGateway countriesDataTableGateway)
            {
                var countries = new List<SelectListItem>();

                foreach (var country in countriesDataTableGateway.GetAll())
                {
                    countries.Add(new SelectListItem()
                    {
                        Value= country.CountryName,
                        Text = country.CountryName

                    }); 
                }
                return countries.ToArray();
            }

            public void UpdatePortfolios(IPortfolioDataTableGateway portfolioDataTableGateway,
                                         ICountriesDataTableGateway countriesDataTableGateway,
                                         PublicWebReportFacade.PortfolioModel[] portfolios)
            {
                var countryDtos = countriesDataTableGateway.GetAll();

                foreach (var portfolio in portfolios)
                {
                    var country = countryDtos.FirstOrDefault(c => c.CountryName == portfolio.Country);

                    int countryId=0;

                    if (country != null)
                    {
                        countryId = country.CountryId;
                    }


                    portfolioDataTableGateway.Update(new PortfolioDto()
                    {
                        Number = portfolio.Number,
                        Name = portfolio.Name,
                        CountryId = countryId,
                        InceptionDate = portfolio.InceptionDate,
                        CloseDate = portfolio.CloseDate
                    });

                    

                }



            }
        }

    }
}