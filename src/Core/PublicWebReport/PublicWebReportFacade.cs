using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PublicWebReportFacade
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;

        public PublicWebReportFacade(
            IPortfolioDataTableGateway portfolioDataTableGateway)
        {
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }

        public PortfolioModel[] GetPortfolioPerforance(
            MonthYear monthYear)
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            var portfolioModels = new List<PortfolioModel>();

            foreach (var portfolioDto in portfolioDtos)
            {
                portfolioModels.Add(new PortfolioModel()
                {
                    Number = portfolioDto.Number,
                    Name = portfolioDto.Name
                });
            }

            return portfolioModels.ToArray();
        }

        public class PortfolioModel
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
    }
}