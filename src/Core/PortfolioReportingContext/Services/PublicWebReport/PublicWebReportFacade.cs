using System.Collections.Generic;
using System.Security.AccessControl;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport
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

        public PortfolioModel[] GetPortfolioPerformance()
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
            var sixMonthCalculationRequest = CalculateReturnRequestFactory.SixMonth(previousMonthYear);
            var quarterToDateCalculationRequest = CalculateReturnRequestFactory.QuarterToDate(previousMonthYear);
            var yearToDateCalculationRequest = CalculateReturnRequestFactory.YearToDate(previousMonthYear);

            var portfolioModel = new PortfolioModel()
            {
                Number = portfolioWithPerformance.Number,
                Name = portfolioWithPerformance.Name,
                //OneMonth = portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear),
                OneMonth = portfolioWithPerformance.GetNetMonthlyReturnPercent(previousMonthYear),
                ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsPercent(threeMonthCalculationRequest),
                SixMonth = portfolioWithPerformance.CalculateNetReturnAsPercent(sixMonthCalculationRequest),
                YearToDate = portfolioWithPerformance.CalculateNetReturnAsPercent(yearToDateCalculationRequest),
                QuarterToDate = portfolioWithPerformance.CalculateNetReturnAsPercent(quarterToDateCalculationRequest),
                StandardDeviation = portfolioWithPerformance.CalculateNetStandardDeviation(),
                Mean = portfolioWithPerformance.CalculateNetMean()
            };

            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();

            foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
            {
                var benchmarkModel = new BenchmarkModel()
                {
                    Name = benchmarkWithPerformance.Name,
                    OneMonth = benchmarkWithPerformance.GetNetMonthlyReturnPercent(previousMonthYear),
                    ThreeMonth = benchmarkWithPerformance.CalculateReturnAsPercent(threeMonthCalculationRequest),
                    SixMonth = benchmarkWithPerformance.CalculateReturnAsPercent(sixMonthCalculationRequest),
                    QuarterToDate = benchmarkWithPerformance.CalculateReturnAsPercent(quarterToDateCalculationRequest),
                    YearToDate = benchmarkWithPerformance.CalculateReturnAsPercent(yearToDateCalculationRequest),
                    StandardDeviation = benchmarkWithPerformance.CalculateStandardDeviation(),
                    Mean = benchmarkWithPerformance.CalculateMean()
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
            public decimal? SixMonth { get; set; }
            public decimal? QuarterToDate { get; set; }
            public decimal? YearToDate { get; set; }
            public decimal? StandardDeviation { get; set; }
            public decimal? Mean { get; set; }

            public BenchmarkModel[] Benchmarks { get; set; }
        }

        public class BenchmarkModel
        {
            public string Name { get; set; }
            public decimal? OneMonth { get; set; }
            public decimal? ThreeMonth { get; set; }
            public decimal? SixMonth { get; set; }
            public decimal? QuarterToDate { get; set; }
            public decimal? YearToDate { get; set; }
            public decimal? StandardDeviation { get; set; }
            public decimal? Mean { get; set; }

        }
    }
}