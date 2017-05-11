using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PortfolioWithPerformanceRepository
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

        private readonly ReturnSeriesRepository _returnSeriesRepository;

        public PortfolioWithPerformanceRepository(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway,
            ReturnSeriesRepository returnSeriesRepository)
        {
            _returnSeriesRepository = returnSeriesRepository;
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }

        public PortfolioWithPerformance[] GetAll()
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            var portfolioModels = new List<PortfolioWithPerformance>();

            var portfolioNumbers = portfolioDtos.Select(p => p.Number).ToArray();

            var portfolioToReturnSeriesDtos = _portfolioToReturnSeriesDataTableGateway.Get(portfolioNumbers);

            var targetReturnSeriesIds = portfolioToReturnSeriesDtos
                .Select(d => d.ReturnSeriesId)
                .ToArray();

            var returnSeries = _returnSeriesRepository.GetReturnSeries(targetReturnSeriesIds);
            
            foreach (var portfolioDto in portfolioDtos)
            {
                var netDto = portfolioToReturnSeriesDtos.FindNet(
                    portfolioDto.Number);

                var grossDto = portfolioToReturnSeriesDtos.FindGross(
                    portfolioDto.Number);

                ReturnSeries netReturnSeries = null;
                ReturnSeries grossReturnSeries = null;

                if (netDto != null)
                    netReturnSeries = returnSeries.FirstOrDefault(r => r.Id == netDto.ReturnSeriesId);

                if (grossDto != null)
                    grossReturnSeries = returnSeries.FirstOrDefault(r => r.Id == grossDto.ReturnSeriesId);

                portfolioModels.Add(new PortfolioWithPerformance(portfolioDto.Number, portfolioDto.Name, netReturnSeries, grossReturnSeries));
            }

            return portfolioModels.ToArray();
        }

    }
}