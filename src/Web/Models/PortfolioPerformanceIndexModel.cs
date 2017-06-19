using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceIndexModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }
        public String SelectedCountry { get; set; }

        public IEnumerable<SelectListItem> NetGrossList { get; set; }
        public string SelectedTypeOfReturn { get; set; }

        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}