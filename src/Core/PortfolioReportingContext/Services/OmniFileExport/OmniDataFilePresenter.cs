using System.Collections.Generic;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.OmniFileExport
{
    public class OmniDataFilePresenter
    {
        private readonly PortfolioWithPerformanceRepository _portfolioWithPerformanceRepository;
        private readonly OmniDataFileView _omniDataFileView;

        public OmniDataFilePresenter(
            PortfolioWithPerformanceRepository portfolioWithPerformanceRepository,
            OmniDataFileView omniDataFileView)
        {
            _omniDataFileView = omniDataFileView;
            _portfolioWithPerformanceRepository = portfolioWithPerformanceRepository;
        }

        public void CreateFile(
            MonthYear endMonth,
            string fullFilePath)
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var models = CreateFileLineModels(endMonth, portfolios);

            _omniDataFileView.CreateDataFile(models, fullFilePath);
        }

        private List<OmniDataFileLineModel> CreateFileLineModels(
            MonthYear endMonth,
            PortfolioWithPerformance[] portfolios)
        {
            var models = new List<OmniDataFileLineModel>();

            var lineFactory = new OmniDataFileLineModelFactory(endMonth);

            foreach (var portfolio in portfolios)
            {
                lineFactory.SetCurrentPortfolio(portfolio);

                models.AddRange(lineFactory.CreateGrossMonthLineModels());
                models.AddRange(lineFactory.CreateNetMonthLineModels());

                models.AddRange(lineFactory.CreateGrossQuarterLineModels());
                models.AddRange(lineFactory.CreateNetQuarterLineModels());

                var grossYearToDateLineModel = lineFactory.CreateGrossYearToDateLineModel();

                if (grossYearToDateLineModel != null)
                    models.Add(grossYearToDateLineModel);

                var netYearToDateLineModel = lineFactory.CreateNetYearToDateLineModel();

                if (netYearToDateLineModel != null)
                    models.Add(netYearToDateLineModel);
            }

            return models;
        }

    }
}