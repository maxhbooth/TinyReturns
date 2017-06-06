using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceNetGrossModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }
        public IEnumerable<SelectListItem> NetGrossList { get; set; }
        public string Selected { get; set; }
    }
}