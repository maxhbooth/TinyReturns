using System.Collections.Generic;
using Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.OmniFileExport
{
    public class OmniDataFileView
    {
        private readonly IFlatFileIo _flatFileIo;

        public OmniDataFileView(
            IFlatFileIo flatFileIo)
        {
            _flatFileIo = flatFileIo;
        }

        public void CreateDataFile(
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