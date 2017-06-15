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
            var oneMonthCalculationRequest = CalculateReturnRequestFactory.OneMonth(previousMonthYear);
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
                    NetGrowthOfWealth = portfolioWithPerformance.NetGrowthofWealthSeries,
                    GrossGrowthOfWealth = portfolioWithPerformance.GrossGrowthofWealthSeries,
                    OneMonth = portfolioWithPerformance.CalculateNetReturn(oneMonthCalculationRequest).AsPercent(),
                    ThreeMonth = portfolioWithPerformance.CalculateNetReturn(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = portfolioWithPerformance.CalculateNetReturn(sixMonthCalculationRequest).AsPercent(),
                    YearToDate = portfolioWithPerformance.CalculateNetReturn(yearToDateCalculationRequest).AsPercent(),
                    QuarterToDate = portfolioWithPerformance.CalculateNetReturn(quarterToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = portfolioWithPerformance.CalculateNetReturn(firstFullMonthCalculationRequest).AsPercent(),
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
                    NetGrowthOfWealth = portfolioWithPerformance.NetGrowthofWealthSeries,
                    GrossGrowthOfWealth = portfolioWithPerformance.GrossGrowthofWealthSeries,
                    OneMonth = portfolioWithPerformance.CalculateGrossReturn(oneMonthCalculationRequest).AsPercent(),
                    ThreeMonth = portfolioWithPerformance.CalculateGrossReturn(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = portfolioWithPerformance.CalculateGrossReturn(sixMonthCalculationRequest).AsPercent(),
                    QuarterToDate = portfolioWithPerformance.CalculateGrossReturn(quarterToDateCalculationRequest).AsPercent(),
                    YearToDate = portfolioWithPerformance.CalculateGrossReturn(yearToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = portfolioWithPerformance.CalculateGrossReturn(firstFullMonthCalculationRequest).AsPercent(),

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
                    OneMonth = benchmarkWithPerformance.CalculateReturn(oneMonthCalculationRequest).AsPercent(),
                    ThreeMonth = benchmarkWithPerformance.CalculateReturn(threeMonthCalculationRequest).AsPercent(),
                    SixMonth = benchmarkWithPerformance.CalculateReturn(sixMonthCalculationRequest).AsPercent(),
                    QuarterToDate = benchmarkWithPerformance.CalculateReturn(quarterToDateCalculationRequest).AsPercent(),
                    YearToDate = benchmarkWithPerformance.CalculateReturn(yearToDateCalculationRequest).AsPercent(),
                    FirstFullMonth = benchmarkWithPerformance.CalculateReturn(firstFullMonthCalculationRequest).AsPercent(),
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

            public ReturnResult OneMonth { get; set; }
            public ReturnResult ThreeMonth { get; set; }
            public ReturnResult SixMonth { get; set; }
            public ReturnResult QuarterToDate { get; set; }
            public ReturnResult YearToDate { get; set; }
            public ReturnResult FirstFullMonth { get; set; }
            public decimal? StandardDeviation { get; set; }
            public decimal? Mean { get; set; }

            public string Country { get; set; }

            public GrowthofWealthSeries NetGrowthOfWealth { get; set; }
            public GrowthofWealthSeries GrossGrowthOfWealth { get; set; }

            public BenchmarkModel[] Benchmarks { get; set; }

            public DateTime InceptionDate { get; set; }
            public DateTime? CloseDate { get; set; }
        }

        public class BenchmarkModel
        {
            public string Name { get; set; }
            public ReturnResult OneMonth { get; set; }
            public ReturnResult ThreeMonth { get; set; }
            public ReturnResult SixMonth { get; set; }
            public ReturnResult QuarterToDate { get; set; }
            public ReturnResult YearToDate { get; set; }
            public ReturnResult FirstFullMonth { get; set; }

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
            return decimal.Round(decimalToChange * 100, 2, MidpointRounding.AwayFromZero);
        }


        public static ReturnResult AsPercent(this ReturnResult ReturnResultToChange)
        {
            return ReturnResult.CreateWithValue(
                decimal.Round(ReturnResultToChange.Value * 100, 2, MidpointRounding.AwayFromZero),
                ReturnResultToChange.Calculation);

        }
    }
}