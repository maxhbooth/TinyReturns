using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.FlatFiles;
using Dimensional.TinyReturns.Core.PublicWebReport;

namespace Dimensional.TinyReturns.Core.OmniFileExport
{
    public class OmniDataFileCreator
    {
        private readonly IFlatFileIo _flatFileIo;
        private readonly PortfolioWithPerformanceRepository _portfolioWithPerformanceRepository;

        public OmniDataFileCreator(
            PortfolioWithPerformanceRepository portfolioWithPerformanceRepository,
            IFlatFileIo flatFileIo)
        {
            _portfolioWithPerformanceRepository = portfolioWithPerformanceRepository;
            _flatFileIo = flatFileIo;
        }

        public void CreateFile(
            MonthYear endMonth,
            string fullFilePath)
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var models = CreateFileLineModels(endMonth, portfolios);

            CreateDataFile(models, fullFilePath);
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

        private void CreateDataFile(
            List<OmniDataFileLineModel> models,
            string fullFilePath)
        {
            var flatFile = new FlatFile<OmniDataFileLineModel>(_flatFileIo);

            flatFile
                .SetDelimiter("|")
                .AddColumn(c => c.InvestmentVehicleId, o => o.Heading("Fund Id"))
                .AddColumn(c => c.Name)
                .AddColumn(c => c.FeeType, o => o.Heading("Fee Type"))
                .AddColumn(c => c.Duration)
                .AddColumn(c => c.EndDate, o => o.Heading("End Date"))
                .AddColumn(c => c.ReturnValue, o => o.Heading("Return Value"))
                .WriteFile(fullFilePath, models.ToArray());
        }
    }
}