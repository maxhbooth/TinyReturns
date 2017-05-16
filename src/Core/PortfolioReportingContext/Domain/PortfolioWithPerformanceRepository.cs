using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class PortfolioWithPerformanceRepository
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
        private readonly IPortfolioToBenchmarkDataTableGateway _portfolioToBenchmarkDataTableGateway;

        private readonly ReturnSeriesRepository _returnSeriesRepository;
        private readonly BenchmarkWithPerformanceRepository _benchmarkWithPerformanceRepository;

        public PortfolioWithPerformanceRepository(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway,
            IPortfolioToBenchmarkDataTableGateway portfolioToBenchmarkDataTableGateway,
            ReturnSeriesRepository returnSeriesRepository,
            BenchmarkWithPerformanceRepository benchmarkWithPerformanceRepository)
        {
            _portfolioToBenchmarkDataTableGateway = portfolioToBenchmarkDataTableGateway;
            _benchmarkWithPerformanceRepository = benchmarkWithPerformanceRepository;
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

            var benchmarkWithPerformances = _benchmarkWithPerformanceRepository.GetAll();
            var portfolioToBenchmarkDtos = _portfolioToBenchmarkDataTableGateway.GetAll();

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

                var benchmarkNumbers = portfolioToBenchmarkDtos
                    .Where(d => d.PortfolioNumber == portfolioDto.Number)
                    .Select(b => b.BenchmarkNumber)
                    .ToArray();

                var withPerformances = benchmarkWithPerformances.Where(b => benchmarkNumbers.Any(n => n == b.Number)).ToArray();

                portfolioModels.Add(new PortfolioWithPerformance(portfolioDto.Number, portfolioDto.Name, netReturnSeries, grossReturnSeries, withPerformances));
            }

            return portfolioModels.ToArray();
        }

    }
}