using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceNetGrossModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }
    }
}