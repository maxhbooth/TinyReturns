using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.FlatFiles;

namespace Dimensional.TinyReturns.Core.OmniFileExport
{
    public class OmniDataFileCreator
    {
        private readonly IFlatFileIo _flatFileIo;
        private readonly InvestmentVehicleReturnsRepository _investmentVehicleReturnsRepository;

        public OmniDataFileCreator(
            InvestmentVehicleReturnsRepository investmentVehicleReturnsRepository,
            IFlatFileIo flatFileIo)
        {
            _investmentVehicleReturnsRepository = investmentVehicleReturnsRepository;
            _flatFileIo = flatFileIo;
        }

        public void CreateFile(
            MonthYear endMonth)
        {
            var portfolios = _investmentVehicleReturnsRepository.GetPortfolios();

            var models = new List<OmniDataFileLineModel>();

            var lineFactory = new OmniDataFileLineModelFactory(endMonth);

            if (portfolios.Any())
            {
                foreach (var portfolio in portfolios)
                {
                    lineFactory.SetCurrentPortfolio(portfolio);

                    models.AddRange(lineFactory.CreateMonthLineModels(FeeType.NetOfFees));
                    models.AddRange(lineFactory.CreateMonthLineModels(FeeType.GrossOfFees));

                    models.AddRange(lineFactory.CreateQuarterLineModels(FeeType.GrossOfFees));
                    models.AddRange(lineFactory.CreateQuarterLineModels(FeeType.NetOfFees));

                    var grossYearToDateLineModel = lineFactory.CreateYearToDateLineModel(FeeType.GrossOfFees);

                    if (grossYearToDateLineModel != null)
                        models.Add(grossYearToDateLineModel);

                    var netYearToDateLineModel = lineFactory.CreateYearToDateLineModel(FeeType.NetOfFees);

                    if (netYearToDateLineModel != null)
                        models.Add(netYearToDateLineModel);
                }
            }
            
            CreateDataFile(models);
        }

        private void CreateDataFile(List<OmniDataFileLineModel> models)
        {
            var flatFile = new FlatFile<OmniDataFileLineModel>(_flatFileIo);

            flatFile
                .SetDelimiter("|")
                .AddColumn(c => c.InvestmentVehicleId, o => o.Heading("Investment Vehicle Id"))
                .AddColumn(c => c.Type)
                .AddColumn(c => c.Name)
                .AddColumn(c => c.FeeType, o => o.Heading("Fee Type"))
                .AddColumn(c => c.Duration)
                .AddColumn(c => c.EndDate, o => o.Heading("End Date"))
                .AddColumn(c => c.ReturnValue, o => o.Heading("Return Value"))
                .WriteFile("c:\\temp\\foo.dat", models.ToArray());
        }
    }
}