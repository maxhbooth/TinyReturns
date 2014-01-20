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
            MonthYear monthYear)
        {
            var entitiesWithReturnSeries = _investmentVehicleReturnsRepository.GetEntitiesWithReturnSeries();

            var models = new List<OmniDataFileLineModel>();

            if (entitiesWithReturnSeries.Any())
            {
                foreach (var portfolio in entitiesWithReturnSeries)
                {
                    var monthlyReturnSeries = portfolio.GetAllReturnSeries().FirstOrDefault(s => s.FeeType == FeeType.NetOfFees);

                    var allMonthlyReturns = monthlyReturnSeries.GetAllMonthlyReturns();

                    foreach (var r in allMonthlyReturns)
                    {
                        models.Add(new OmniDataFileLineModel()
                        {
                            InvestmentVehicleId = portfolio.EntityNumber.ToString(),
                            Type = "Portfolio",
                            Name = portfolio.Name,
                            FeeType = FeeType.NetOfFees.Code.ToString(),
                            Duration = "M",
                            EndDate = r.MonthYear.LastDayOfMonth.ToString("yyyy-M-d"),
                            ReturnValue = r.ReturnValue.ToString()
                        });
                    }
                }
            }
            
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