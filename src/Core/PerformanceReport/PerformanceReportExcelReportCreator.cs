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

            recordModels.Add(CreateRecordModel(portfolio, monthYear));

            var reportModel = new PerformanceReportExcelReportModel();

            reportModel.MonthText = string.Format("Month: {0}/{1}", monthYear.Month, monthYear.Year);
            reportModel.Records = recordModels.ToArray();

            _view.RenderReport(reportModel);
        }

        private PerformanceReportExcelReportRecordModel CreateRecordModel(
            InvestmentVehicle portfolio,
            MonthYear monthYear)
        {
            var recordModel = new PerformanceReportExcelReportRecordModel()
            {
                Name = portfolio.Name,
                Type = "Portfolio",
                FeeType = FeeType.NetOfFees.DisplayName,
            };

            var oneMonthRequest = CalculateReturnRequestFactory.OneMonth(monthYear);
            var oneMonthResult = portfolio.CalculateReturn(oneMonthRequest, FeeType.NetOfFees);
            recordModel.OneMonth = oneMonthResult.Value;

            var threeMonthRequest = CalculateReturnRequestFactory.ThreeMonth(monthYear);
            var threeMonthResult = portfolio.CalculateReturn(threeMonthRequest, FeeType.NetOfFees);
            recordModel.ThreeMonths = threeMonthResult.Value;

            var twelveMonthRequest = CalculateReturnRequestFactory.TwelveMonth(monthYear);
            var twelveMonthResult = portfolio.CalculateReturn(twelveMonthRequest, FeeType.NetOfFees);
            recordModel.TwelveMonths = twelveMonthResult.Value;

            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(monthYear);
            var yearToDateResult = portfolio.CalculateReturn(yearToDateRequest, FeeType.NetOfFees);
            recordModel.YearToDate = yearToDateResult.Value;

            return recordModel;
        }
    }
}