using System.Collections.Generic;
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

            var returnSeriesDtos = new List<ReturnSeriesDto>();

            foreach (var portfolioDto in portfolioDtos)
            {
                returnSeriesDtos.Add(new ReturnSeriesDto()
                {
                    Name = CreateReturnSeriesName(portfolioDto.Name),
                    Disclosure = string.Empty
                });
            }

            _returnSeriesDataTableGateway.Insert(returnSeriesDtos.ToArray());
        }

        private string CreateReturnSeriesName(string portfolioName)
        {
            return string.Format("Returns for {0}", portfolioName);
        }
    }
}