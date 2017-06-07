using System;
using System.Collections.Generic;
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

        public PortfolioModel[] GetGrossPortfolioPerforance()
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var currentMonthYear = new MonthYear(_clock.GetCurrentDate());
            var previousMonthYear = currentMonthYear.AddMonths(-1);

            var portfolioModels = new List<PortfolioModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var portfolioModel = CreateGrossPortfolioModel(portfolioWithPerformance, previousMonthYear);

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
                OneMonth = PercentHelper.AsPercent(portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear)),
                ThreeMonth = PercentHelper.AsPercent(portfolioWithPerformance.CalculateNetReturnAsDecimal(threeMonthCalculationRequest)),
                SixMonth = PercentHelper.AsPercent(portfolioWithPerformance.CalculateNetReturnAsDecimal(sixMonthCalculationRequest)),
                YearToDate = PercentHelper.AsPercent(portfolioWithPerformance.CalculateNetReturnAsDecimal(yearToDateCalculationRequest)),
                QuarterToDate = PercentHelper.AsPercent(portfolioWithPerformance.CalculateNetReturnAsDecimal(quarterToDateCalculationRequest)),
                NetNotGross = true
            };

            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();

            foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
            {
                var benchmarkModel = new BenchmarkModel()
                {
                    Name = benchmarkWithPerformance.Name,
                    OneMonth = PercentHelper.AsPercent(benchmarkWithPerformance.GetNetMonthlyReturn(previousMonthYear)),
                    ThreeMonth = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(threeMonthCalculationRequest)),
                    SixMonth = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(sixMonthCalculationRequest)),
                    QuarterToDate = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(quarterToDateCalculationRequest)),
                    YearToDate = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(yearToDateCalculationRequest)),
                };
                benchmarkModels.Add(benchmarkModel);
            }

            portfolioModel.Benchmarks = benchmarkModels.ToArray();

            return portfolioModel;
        }
        private static PortfolioModel CreateGrossPortfolioModel(
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
                OneMonth = PercentHelper.AsPercent(portfolioWithPerformance.GetGrossMonthlyReturn(previousMonthYear)),
                ThreeMonth = PercentHelper.AsPercent(portfolioWithPerformance.CalculateGrossReturnAsDecimal(threeMonthCalculationRequest)),
                SixMonth = PercentHelper.AsPercent(portfolioWithPerformance.CalculateGrossReturnAsDecimal(sixMonthCalculationRequest)),
                QuarterToDate = PercentHelper.AsPercent(portfolioWithPerformance.CalculateGrossReturnAsDecimal(quarterToDateCalculationRequest)),
                YearToDate = PercentHelper.AsPercent(portfolioWithPerformance.CalculateGrossReturnAsDecimal(yearToDateCalculationRequest)),
                NetNotGross = false
            };

            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();

            foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
            {
                var benchmarkModel = new BenchmarkModel()
                {
                    //benchmarks only have net; no gross returns
                    Name = benchmarkWithPerformance.Name,
                    OneMonth = PercentHelper.AsPercent(benchmarkWithPerformance.GetNetMonthlyReturn(previousMonthYear)),
                    ThreeMonth = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(threeMonthCalculationRequest)),
                    SixMonth = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(sixMonthCalculationRequest)),
                    QuarterToDate = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(quarterToDateCalculationRequest)),
                    YearToDate = PercentHelper.AsPercent(benchmarkWithPerformance.CalculateReturnAsDecimal(yearToDateCalculationRequest))
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
            public bool NetNotGross { get; set; }

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
        }
    }

    public static class PercentHelper
    {
        public static decimal? AsPercent(decimal? decimalToChange)
        {
            if (decimalToChange == null)
            {
                return null;
            }
            return decimal.Round(decimalToChange.Value * 100, 2, MidpointRounding.AwayFromZero);
        }
        public static decimal AsPercent(decimal decimalToChange)
        {

            return decimal.Round(decimalToChange* 100, 2, MidpointRounding.AwayFromZero);
        }
    }
}