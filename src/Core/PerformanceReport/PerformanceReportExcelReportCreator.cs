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

            recordModels.Add(CreateRecordModel(portfolio, monthYear, FeeType.NetOfFees));

            var reportModel = new PerformanceReportExcelReportModel();

            reportModel.MonthText = string.Format("Month: {0}/{1}", monthYear.Month, monthYear.Year);
            reportModel.Records = recordModels.ToArray();

            _view.RenderReport(reportModel);
        }

        private PerformanceReportExcelReportRecordModel CreateRecordModel(
            InvestmentVehicle portfolio,
            MonthYear monthYear,
            FeeType feeType)
        {
            var recordModel = new PerformanceReportExcelReportRecordModel()
            {
                EntityNumber = portfolio.Number,
                Name = portfolio.Name,
                Type = "Portfolio",
                FeeType = feeType.DisplayName,
            };

            var oneMonthRequest = CalculateReturnRequestFactory.OneMonth(monthYear);
            var oneMonthResult = portfolio.CalculateReturn(oneMonthRequest, feeType);
            recordModel.OneMonth = oneMonthResult.GetValueNullOnError();

            var threeMonthRequest = CalculateReturnRequestFactory.ThreeMonth(monthYear);
            var threeMonthResult = portfolio.CalculateReturn(threeMonthRequest, feeType);
            recordModel.ThreeMonths = threeMonthResult.GetValueNullOnError();

            var twelveMonthRequest = CalculateReturnRequestFactory.TwelveMonth(monthYear);
            var twelveMonthResult = portfolio.CalculateReturn(twelveMonthRequest, feeType);
            recordModel.TwelveMonths = twelveMonthResult.GetValueNullOnError();

            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(monthYear);
            var yearToDateResult = portfolio.CalculateReturn(yearToDateRequest, feeType);
            recordModel.YearToDate = yearToDateResult.GetValueNullOnError();

            return recordModel;
        }
    }
}