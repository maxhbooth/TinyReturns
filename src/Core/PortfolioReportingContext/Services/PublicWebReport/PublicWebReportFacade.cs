using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

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
            var currentMonthYear = new MonthYear(_clock.GetCurrentDate());

            var previousMonthYear = currentMonthYear.AddMonths(-1);

            return GetPortfolioPerformance(previousMonthYear);
        }

        public PortfolioModel[] GetPortfolioPerformance(MonthYear currentMonthYear)
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var portfolioModels = new List<PortfolioModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var portfolioModel = CreatePortfolioModel(portfolioWithPerformance, currentMonthYear, true);

                portfolioModels.Add(portfolioModel);
            }

            return portfolioModels.ToArray();
        }

        public PortfolioModel[] GetGrossPortfolioPerformance(MonthYear currentMonthYear)
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var portfolioModels = new List<PortfolioModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var portfolioModel = CreatePortfolioModel(portfolioWithPerformance, currentMonthYear, false);

                portfolioModels.Add(portfolioModel);
            }

            return portfolioModels.ToArray();
        }
        private static PortfolioModel CreatePortfolioModel(
            PortfolioWithPerformance portfolioWithPerformance,
            MonthYear previousMonthYear, bool netReturn)
        {
            var firstFullMonth = new MonthYear(portfolioWithPerformance.InceptionDate).AddMonths(1);
            int fullMonthsSinceInception = new MonthYearRange(firstFullMonth, previousMonthYear).NumberOfMonthsInRange;
            var threeMonthCalculationRequest = CalculateReturnRequestFactory.ThreeMonth(previousMonthYear);
            var sixMonthCalculationRequest = CalculateReturnRequestFactory.SixMonth(previousMonthYear);
            var quarterToDateCalculationRequest = CalculateReturnRequestFactory.QuarterToDate(previousMonthYear);
            var yearToDateCalculationRequest = CalculateReturnRequestFactory.YearToDate(previousMonthYear);
            var firstFullMonthCalculationRequest = CalculateReturnRequestFactory.FirstFullMonth(previousMonthYear, fullMonthsSinceInception);
            PortfolioModel portfolioModel = null;

            if (netReturn)
            {
                portfolioModel = new PortfolioModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    Country = portfolioWithPerformance.Country,
                    OneMonth = portfolioWithPerformance.GetNetMonthlyReturn(previousMonthYear).AsPercent(),
                    ThreeMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(sixMonthCalculationRequest).AsPercent(),
                    YearToDate = portfolioWithPerformance.CalculateNetReturnAsDecimal(yearToDateCalculationRequest).AsPercent(),
                    QuarterToDate = portfolioWithPerformance.CalculateNetReturnAsDecimal(quarterToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = portfolioWithPerformance.CalculateNetReturnAsDecimal(firstFullMonthCalculationRequest).AsPercent(),
                    StandardDeviation = portfolioWithPerformance.CalculateNetStandardDeviation().AsPercent(),  
                    Mean = portfolioWithPerformance.CalculateNetMean().AsPercent(),

                    InceptionDate = portfolioWithPerformance.InceptionDate,
                    CloseDate = portfolioWithPerformance.CloseDate
                };
            }
            else if(!netReturn)
            {
                portfolioModel = new PortfolioModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    Country = portfolioWithPerformance.Country,
                    OneMonth = portfolioWithPerformance.GetGrossMonthlyReturn(previousMonthYear).AsPercent(),
                    ThreeMonth = portfolioWithPerformance.CalculateGrossReturnAsDecimal(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = portfolioWithPerformance.CalculateGrossReturnAsDecimal(sixMonthCalculationRequest).AsPercent(),
                    QuarterToDate = portfolioWithPerformance.CalculateGrossReturnAsDecimal(quarterToDateCalculationRequest).AsPercent(),
                    YearToDate = portfolioWithPerformance.CalculateGrossReturnAsDecimal(yearToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = portfolioWithPerformance.CalculateGrossReturnAsDecimal(firstFullMonthCalculationRequest).AsPercent(),
                    StandardDeviation = portfolioWithPerformance.CalculateGrossStandardDeviation().AsPercent(),
                    Mean = portfolioWithPerformance.CalculateGrossMean().AsPercent(),

                    InceptionDate = portfolioWithPerformance.InceptionDate,
                    CloseDate = portfolioWithPerformance.CloseDate
                };
            }
            
            var benchmarkModels = new List<BenchmarkModel>();

            var benchmarkWithPerformances = portfolioWithPerformance.GetAllBenchmarks();

            foreach (var benchmarkWithPerformance in benchmarkWithPerformances)
            {
                var benchmarkModel = new BenchmarkModel()
                {
                    Name = benchmarkWithPerformance.Name,
                    OneMonth = benchmarkWithPerformance.GetNetMonthlyReturn(previousMonthYear).AsPercent(),
                    ThreeMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(sixMonthCalculationRequest).AsPercent(),
                    QuarterToDate = benchmarkWithPerformance.CalculateReturnAsDecimal(quarterToDateCalculationRequest).AsPercent(),
                    YearToDate = benchmarkWithPerformance.CalculateReturnAsDecimal(yearToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = benchmarkWithPerformance.CalculateReturnAsDecimal(firstFullMonthCalculationRequest).AsPercent(),
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
            public decimal? StandardDeviation { get; set; }
            public decimal? Mean { get; set; }

            public string Country { get; set; }

            public BenchmarkModel[] Benchmarks { get; set; }

            public DateTime InceptionDate { get; set; }
            public DateTime? CloseDate { get; set; }
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
            public decimal? StandardDeviation { get; set; }
            public decimal? Mean { get; set; }

        }
    }

    public static class PercentHelper
    {
        public static decimal? AsPercent(this decimal? decimalToChange)
        {
            if (decimalToChange == null)
            {
                return null;
            }
            return decimalToChange.Value.AsPercent();
        }
        public static decimal AsPercent(this decimal decimalToChange)
        {
            return decimal.Round(decimalToChange* 100, 2, MidpointRounding.AwayFromZero);
        }
    }
}