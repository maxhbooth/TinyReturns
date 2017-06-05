﻿using System.Collections.Generic;
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
            var firstFullMonth = new MonthYear(portfolioWithPerformance.InceptionDate).AddMonths(1);
            int fullMonthsSinceInception = new MonthYearRange(firstFullMonth, previousMonthYear).NumberOfMonthsInRange;

            var threeMonthCalculationRequest = CalculateReturnRequestFactory.ThreeMonth(previousMonthYear);
            var sixMonthCalculationRequest = CalculateReturnRequestFactory.SixMonth(previousMonthYear);
            var quarterToDateCalculationRequest = CalculateReturnRequestFactory.QuarterToDate(previousMonthYear);
            var yearToDateCalculationRequest = CalculateReturnRequestFactory.YearToDate(previousMonthYear);
            var firstFullMonthCalculationRequest = CalculateReturnRequestFactory.FirstFullMonth(previousMonthYear, fullMonthsSinceInception);
            var portfolioModel = new PortfolioModel()
            {
                Number = portfolioWithPerformance.Number,
                Name = portfolioWithPerformance.Name,
                OneMonth = portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear),
                ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(threeMonthCalculationRequest),
                SixMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(sixMonthCalculationRequest),
                YearToDate = portfolioWithPerformance.CalculateNetReturnAsDecimal(yearToDateCalculationRequest),
                QuarterToDate = portfolioWithPerformance.CalculateNetReturnAsDecimal(quarterToDateCalculationRequest),
                FirstFullMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(firstFullMonthCalculationRequest),
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
                    SixMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(sixMonthCalculationRequest),
                    QuarterToDate = benchmarkWithPerformance.CalculateReturnAsDecimal(quarterToDateCalculationRequest),
                    YearToDate = benchmarkWithPerformance.CalculateReturnAsDecimal(yearToDateCalculationRequest),
                    FirstFullMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(firstFullMonthCalculationRequest),
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
            public decimal? FirstFullMonth { get; set; }

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
            public decimal? FirstFullMonth { get; set; }
        }
    }
}