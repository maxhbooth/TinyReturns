using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PublicWebReportFacade
    {
        public class PortfolioModel
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }

        public PortfolioModel[] GetPortfolioPerforance(
            MonthYear monthYear)
        {
            return new PortfolioModel[0];
        }
    }
}