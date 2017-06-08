using System.Collections.Generic;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PastMonthsModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios;
        public string MonthYear { get; set; }
        public IEnumerable<SelectListItem> MonthYears { get; set; }
        public MonthYear currentMonth { get; set; }

    }
}