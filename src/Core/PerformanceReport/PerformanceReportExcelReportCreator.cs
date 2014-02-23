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
            MonthYear monthYear,
            string fullFilePath)
        {
            var investmentVehicles = _returnsRepository.GetAllInvestmentVehicles();

            var portfolioRecords = CreatePortfolioRecords(monthYear, investmentVehicles);
            var benchmarkRecords = CreateBenchmarkRecords(monthYear, investmentVehicles);

            var reportModel = new PerformanceReportExcelReportModel();

            reportModel.MonthText = string.Format("Month: {0}/{1}", monthYear.Month, monthYear.Year);
            reportModel.Records = portfolioRecords.Union(benchmarkRecords).ToArray();
            
            _view.RenderReport(reportModel, fullFilePath);
        }

        private IEnumerable<PerformanceReportExcelReportRecordModel> CreatePortfolioRecords(
            MonthYear monthYear,
            IEnumerable<InvestmentVehicle> investmentVehicles)
        {
            var recordModels = new List<PerformanceReportExcelReportRecordModel>();

            var portfolios = investmentVehicles.Where(i => i.InvestmentVehicleType == InvestmentVehicleType.Portfolio);

            foreach (var portfolio in portfolios)
            {
                recordModels.Add(CreateRecordModel(portfolio, monthYear, FeeType.NetOfFees, "Portfolio"));
                recordModels.Add(CreateRecordModel(portfolio, monthYear, FeeType.GrossOfFees, "Portfolio"));
            }

            return recordModels;
        }

        private IEnumerable<PerformanceReportExcelReportRecordModel> CreateBenchmarkRecords(
            MonthYear monthYear,
            IEnumerable<InvestmentVehicle> investmentVehicles)
        {
            var recordModels = new List<PerformanceReportExcelReportRecordModel>();

            var benchmarks = investmentVehicles.Where(i => i.InvestmentVehicleType == InvestmentVehicleType.Benchmark);

            foreach (var benchmark in benchmarks)
                recordModels.Add(CreateRecordModel(benchmark, monthYear, FeeType.None, "Benchmark"));

            return recordModels;
        }

        private PerformanceReportExcelReportRecordModel CreateRecordModel(
            InvestmentVehicle portfolio,
            MonthYear monthYear,
            FeeType feeType,
            string entityType)
        {
            var recordModel = new PerformanceReportExcelReportRecordModel()
            {
                EntityNumber = portfolio.Number,
                Name = portfolio.Name,
                Type = entityType,
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