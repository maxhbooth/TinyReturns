using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PastMonthsModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios;
        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
    }
}