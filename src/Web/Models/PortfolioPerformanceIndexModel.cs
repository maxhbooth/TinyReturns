using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;

namespace Dimensional.TinyReturns.Web.Models
{
    public class PortfolioPerformanceIndexModel
    {
        public PublicWebReportFacade.PortfolioModel[] Portfolios { get; set; }
    }
}