using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class BenchmarkWithPerformanceRepository
    {
        private readonly ReturnSeriesRepository _returnSeriesRepository;
        private readonly IBenchmarkDataTableGateway _benchmarkDataTableGateway;
        private readonly IBenchmarkToReturnSeriesDataTableGateway _benchmarkToReturnSeriesDataTableGateway;

        public BenchmarkWithPerformanceRepository(
            IBenchmarkDataTableGateway benchmarkDataTableGateway,
            IBenchmarkToReturnSeriesDataTableGateway benchmarkToReturnSeriesDataTableGateway,
            ReturnSeriesRepository returnSeriesRepository)
        {
            _benchmarkToReturnSeriesDataTableGateway = benchmarkToReturnSeriesDataTableGateway;
            _benchmarkDataTableGateway = benchmarkDataTableGateway;
            _returnSeriesRepository = returnSeriesRepository;
        }

        public BenchmarkWithPerformance[] GetAll()
        {
            var benchmarkDtos = _benchmarkDataTableGateway.GetAll();

            var benchmarkToReturnSeriesDtos = _benchmarkToReturnSeriesDataTableGateway.GetAll();

            var returnSeriesIds = benchmarkToReturnSeriesDtos
                .Select(d => d.ReturnSeriesId)
                .ToArray();

            var allReturnSeries = _returnSeriesRepository.GetReturnSeries(returnSeriesIds);

            var benchmarkWithPerformances = new List<BenchmarkWithPerformance>();

            foreach (var benchmarkDto in benchmarkDtos)
            {
                ReturnSeries targetReturnSeries = null;

                var benchmarkToReturnSeriesDto = benchmarkToReturnSeriesDtos.FirstOrDefault(d => d.BenchmarkNumber == benchmarkDto.Number);

                if (benchmarkToReturnSeriesDto != null)
                {
                    targetReturnSeries = allReturnSeries.FirstOrDefault(r => r.Id == benchmarkToReturnSeriesDto.ReturnSeriesId);
                }

                var benchmarkWithPerformance = new BenchmarkWithPerformance(
                    benchmarkDto.Number,
                    benchmarkDto.Name,
                    targetReturnSeries);

                benchmarkWithPerformances.Add(benchmarkWithPerformance);
            }

            return benchmarkWithPerformances.ToArray();
        }
    }
}