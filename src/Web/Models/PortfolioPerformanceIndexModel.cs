using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceIndexModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }

        public IEnumerable<SelectListItem> NetGrossList { get; set; }
        public string Selected { get; set; }

        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
    }
}