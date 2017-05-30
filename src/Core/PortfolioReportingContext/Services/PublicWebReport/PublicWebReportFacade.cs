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
                //OneMonth = portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear),
                OneMonth = portfolioWithPerformance.GetNetMonthlyReturnPercent(previousMonthYear),
                ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsPercent(threeMonthCalculationRequest),
                YearToDate = portfolioWithPerformance.CalculateNetReturnAsPercent(yearToDateCalculationRequest)
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
                    YearToDate = benchmarkWithPerformance.CalculateReturnAsPercent(yearToDateCalculationRequest)
                };

                benchmarkModels.Add(benchmarkModel);
            }

            portfolioModel.Benchmarks = benchmarkModels.ToArray();

            return portfolioModel;
        }

        //this  needs boolean for net or gross
        private static PortfolioModel CreatePortfolioModel(
            PortfolioWithPerformance portfolioWithPerformance,
            MonthYear previousMonthYear, bool net)
        {
            PortfolioModel portfolioModel = new PortfolioModel();
            var threeMonthCalculationRequest = CalculateReturnRequestFactory.ThreeMonth(previousMonthYear);
            var yearToDateCalculationRequest = CalculateReturnRequestFactory.YearToDate(previousMonthYear);
            if (net == true)
            {
                portfolioModel = new PortfolioModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    OneMonth = portfolioWithPerformance.GetNetMonthlyReturnPercent(previousMonthYear),
                    ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsPercent(threeMonthCalculationRequest),
                    YearToDate = portfolioWithPerformance.CalculateNetReturnAsPercent(yearToDateCalculationRequest)
                };
            }
            else
            {
                portfolioModel = new PortfolioModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    OneMonth = portfolioWithPerformance.GetGrossMonthlyReturnPercent(previousMonthYear),
                    ThreeMonth = portfolioWithPerformance.CalculateGrossReturnAsPercent(threeMonthCalculationRequest),
                    YearToDate = portfolioWithPerformance.CalculateGrossReturnAsPercent(yearToDateCalculationRequest)
                };
            }
           

            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();
            if (net == true)
            {
                foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
                {
                    var benchmarkModel = new BenchmarkModel()
                    {
                        Name = benchmarkWithPerformance.Name,
                        OneMonth = benchmarkWithPerformance.GetNetMonthlyReturnPercent(previousMonthYear),
                        ThreeMonth = benchmarkWithPerformance.CalculateReturnAsPercent(threeMonthCalculationRequest),
                        YearToDate = benchmarkWithPerformance.CalculateReturnAsPercent(yearToDateCalculationRequest)
                    };

                    benchmarkModels.Add(benchmarkModel);
                }
            }
            else
            {
                foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
                {
                    var benchmarkModel = new BenchmarkModel()
                    {
                        Name = benchmarkWithPerformance.Name,
                        OneMonth = benchmarkWithPerformance.GetGrossMonthlyReturnPercent(previousMonthYear),
                        //DO WE CARE ABOUT GROSS HERE? FOR BENCHMARKS
                        ThreeMonth = benchmarkWithPerformance.CalculateReturnAsPercent(threeMonthCalculationRequest),
                        YearToDate = benchmarkWithPerformance.CalculateReturnAsPercent(yearToDateCalculationRequest)
                    };

                    benchmarkModels.Add(benchmarkModel);
                }
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