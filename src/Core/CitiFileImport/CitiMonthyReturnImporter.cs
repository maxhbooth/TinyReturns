using System.Linq;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public class CitiMonthyReturnImporter
    {
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;

        public CitiMonthyReturnImporter(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway)
        {
            _portfolioDataTableGateway = portfolioDataTableGateway;
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
        }

        public void ImportMonthyPortfolioNetReturnsFile(
            string filePath)
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            if (portfolioDtos.Length <= 0)
                return;

            _returnSeriesDataTableGateway.Insert(new ReturnSeriesDto()
            {
                Name = "Returns for Portfolio 100",
                Disclosure = string.Empty
            });
        }
    }
}