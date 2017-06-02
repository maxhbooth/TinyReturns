using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceIndexModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }
        public IEnumerable<SelectListItem> Letters { get; set; }
        public string SelectedLetter { get; set; }
    }
}