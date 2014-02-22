using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.PerformanceReport
{
    public class PerformanceReportExcelReportCreator
    {
        private readonly InvestmentVehicleReturnsRepository _returnsRepository;
        private readonly IPerformanceReportExcelReportView _view;

        public PerformanceReportExcelReportCreator(
            InvestmentVehicleReturnsRepository returnsRepository,
            IPerformanceReportExcelReportView view)
        {
            _view = view;
            _returnsRepository = returnsRepository;
        }

        public void CreateReport(
            MonthYear monthYear)
        {
            var investmentVehicles = _returnsRepository.GetAllInvestmentVehicles();

            var portfolio = investmentVehicles.First(i => i.InvestmentVehicleType == InvestmentVehicleType.Portfolio);

            var recordModels = new List<PerformanceReportExcelReportRecordModel>();

            recordModels.Add(new PerformanceReportExcelReportRecordModel()
            {
                Name = portfolio.Name,
                Type = "Portfolio",
                FeeType = FeeType.NetOfFees.DisplayName
            } );

            var reportModel = new PerformanceReportExcelReportModel();

            reportModel.MonthText = string.Format("Month: {0}/{1}", monthYear.Month, monthYear.Year);
            reportModel.Records = recordModels.ToArray();

            _view.RenderReport(reportModel);
        }
    }
}