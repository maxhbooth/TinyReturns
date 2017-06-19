using System.Collections.Generic;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport
{
    public class PublicWebChartFacade
    {
        private readonly IClock _clock;
        private readonly PortfolioWithPerformanceRepository _portfolioWithPerformanceRepository;

        public PublicWebChartFacade(
            PortfolioWithPerformanceRepository portfolioWithPerformanceRepository,
            IClock clock)
        {
            _portfolioWithPerformanceRepository = portfolioWithPerformanceRepository;
            _clock = clock;
        }

        public ChartModel[] GetNetChartData()
        {
            var currentMonthYear = new MonthYear(_clock.GetCurrentDate());

            var previousMonthYear = currentMonthYear.AddMonths(-1);

            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var chartModels = new List<ChartModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var chartModel = CreateChartModel(portfolioWithPerformance, previousMonthYear, true);

                chartModels.Add(chartModel);
            }

            return chartModels.ToArray();
        }

        public ChartModel[] GetGrossChartData()
        {
            var currentMonthYear = new MonthYear(_clock.GetCurrentDate());

            var previousMonthYear = currentMonthYear.AddMonths(-1);

            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var chartModels = new List<ChartModel>();

            foreach (var portfolioWithPerformance in portfolios)
            {
                var chartModel = CreateChartModel(portfolioWithPerformance, previousMonthYear, false);

                chartModels.Add(chartModel);
            }

            return chartModels.ToArray();
        }

        private static ChartModel CreateChartModel(
            PortfolioWithPerformance portfolioWithPerformance,
            MonthYear previousMonthYear, bool netReturn)
        {
            ChartModel chartModel = null;

            if (netReturn)
            {
                chartModel = new ChartModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    GrowthOfWealth = portfolioWithPerformance.NetGrowthofWealthSeries,
                };
            }
            else
            {
                chartModel = new ChartModel()
                {
                    Number = portfolioWithPerformance.Number,
                    Name = portfolioWithPerformance.Name,
                    GrowthOfWealth = portfolioWithPerformance.GrossGrowthofWealthSeries,
                };
            }

            return chartModel;
        }

        public class ChartModel
        {
            public int Number { get; set; }
            public string Name { get; set; }

            public GrowthofWealthSeries GrowthOfWealth { get; set; }
        }

    }
}