using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PublicWebReportFacade
    {
        private readonly IClock _clock;
        private readonly PortfolioWithPerformanceRepository _portfolioWithPerformanceRepository;

        public PublicWebReportFacade(
            PortfolioWithPerformanceRepository portfolioWithPerformanceRepository,
            IClock clock)
        {
            _portfolioWithPerformanceRepository = portfolioWithPerformanceRepository;
            _clock = clock;
        }

        public PortfolioModel[] GetPortfolioPerforance()
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var currentMonthYear = new MonthYear(_clock.GetCurrentDate());
            var previousMonthYear = currentMonthYear.AddMonths(-1);

            var portfolioModels = new List<PortfolioModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var portfolioModel = CreatePortfolioModel(portfolioWithPerformance, previousMonthYear);

                portfolioModels.Add(portfolioModel);
            }

            return portfolioModels.ToArray();
        }

        private static PortfolioModel CreatePortfolioModel(
            PortfolioWithPerformance portfolioWithPerformance,
            MonthYear previousMonthYear)
        {
            var threeMonthCalculationRequest = CalculateReturnRequestFactory.ThreeMonth(previousMonthYear);
            var yearToDateCalculationRequest = CalculateReturnRequestFactory.YearToDate(previousMonthYear);

            var portfolioModel = new PortfolioModel()
            {
                Number = portfolioWithPerformance.Number,
                Name = portfolioWithPerformance.Name,
                OneMonth = portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear),
                ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(threeMonthCalculationRequest),
                YearToDate = portfolioWithPerformance.CalculateNetReturnAsDecimal(yearToDateCalculationRequest)
            };

            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();

            foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
            {
                var benchmarkModel = new BenchmarkModel()
                {
                    Name = benchmarkWithPerformance.Name,
                    OneMonth = benchmarkWithPerformance.GetNetMonthlyReturn(previousMonthYear),
                    ThreeMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(threeMonthCalculationRequest),
                    YearToDate = benchmarkWithPerformance.CalculateReturnAsDecimal(yearToDateCalculationRequest)
                };

                benchmarkModels.Add(benchmarkModel);
            }

            portfolioModel.Benchmarks = benchmarkModels.ToArray();

            return portfolioModel;
        }

        public class PortfolioModel
        {
            public PortfolioModel()
            {
                Benchmarks = new BenchmarkModel[0];
            }

            public int Number { get; set; }
            public string Name { get; set; }

            public decimal? OneMonth { get; set; }
            public decimal? ThreeMonth { get; set; }
            public decimal? YearToDate { get; set; }

            public BenchmarkModel[] Benchmarks { get; set; }
        }

        public class BenchmarkModel
        {
            public string Name { get; set; }
            public decimal? OneMonth { get; set; }
            public decimal? ThreeMonth { get; set; }
            public decimal? YearToDate { get; set; }
        }
    }
}