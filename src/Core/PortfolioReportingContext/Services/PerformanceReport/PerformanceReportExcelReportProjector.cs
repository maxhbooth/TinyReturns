using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport
{
    public class PerformanceReportExcelReportProjector
    {
        private readonly IPerformanceReportExcelReportView _view;

        private readonly PortfolioWithPerformanceRepository _portfolioWithPerformanceRepository;

        public PerformanceReportExcelReportProjector(
            PortfolioWithPerformanceRepository portfolioWithPerformanceRepository,
            IPerformanceReportExcelReportView view)
        {
            _portfolioWithPerformanceRepository = portfolioWithPerformanceRepository;
            _view = view;
        }

        public void CreateReport(
            MonthYear monthYear,
            string fullFilePath)
        {
            var portfolios = _portfolioWithPerformanceRepository.GetAll();

            var portfolioRecords = CreatePortfolioExcelRecords(monthYear, portfolios);

            var reportModel = new PerformanceReportExcelReportModel();

            reportModel.MonthText = string.Format("Month: {0}/{1}", monthYear.Month, monthYear.Year);
            reportModel.Records = portfolioRecords.ToArray();
            
            _view.RenderReport(reportModel, fullFilePath);
        }

        private IEnumerable<PerformanceReportExcelReportRecordModel> CreatePortfolioExcelRecords(
            MonthYear monthYear,
            PortfolioWithPerformance[] portfolios)
        {
            var recordModels = new List<PerformanceReportExcelReportRecordModel>();

            foreach (var portfolio in portfolios)
            {
                if (portfolio.HasNetReturnSeries)
                    recordModels.Add(CreateNetExcelRecord(portfolio, monthYear, "Portfolio"));

                if (portfolio.HasGrossReturnSeries)
                    recordModels.Add(CreateGrossExcelRecord(portfolio, monthYear, "Portfolio"));
            }

            return recordModels;
        }
        private PerformanceReportExcelReportRecordModel CreateNetExcelRecord(
            PortfolioWithPerformance portfolio,
            MonthYear monthYear,
            string entityType)
        {
            var recordModel = new PerformanceReportExcelReportRecordModel()
            {
                EntityNumber = portfolio.Number,
                Name = portfolio.Name,
                Type = entityType,
                FeeType = "Net",
            };

            var oneMonthRequest = CalculateReturnRequestFactory.OneMonth(monthYear);
            var oneMonthResult = portfolio.CalculateNetReturn(oneMonthRequest);
            recordModel.OneMonth = oneMonthResult.GetNullValueOnError();

            var threeMonthRequest = CalculateReturnRequestFactory.ThreeMonth(monthYear);
            var threeMonthResult = portfolio.CalculateNetReturn(threeMonthRequest);
            recordModel.ThreeMonths = threeMonthResult.GetNullValueOnError();

            var twelveMonthRequest = CalculateReturnRequestFactory.TwelveMonth(monthYear);
            var twelveMonthResult = portfolio.CalculateNetReturn(twelveMonthRequest);
            recordModel.TwelveMonths = twelveMonthResult.GetNullValueOnError();

            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(monthYear);
            var yearToDateResult = portfolio.CalculateNetReturn(yearToDateRequest);
            recordModel.YearToDate = yearToDateResult.GetNullValueOnError();

            return recordModel;
        }

        private PerformanceReportExcelReportRecordModel CreateGrossExcelRecord(
            PortfolioWithPerformance portfolio,
            MonthYear monthYear,
            string entityType)
        {
            var recordModel = new PerformanceReportExcelReportRecordModel()
            {
                EntityNumber = portfolio.Number,
                Name = portfolio.Name,
                Type = entityType,
                FeeType = "Gross",
            };

            var oneMonthRequest = CalculateReturnRequestFactory.OneMonth(monthYear);
            var oneMonthResult = portfolio.CalculateGrossReturn(oneMonthRequest);
            recordModel.OneMonth = oneMonthResult.GetNullValueOnError();

            var threeMonthRequest = CalculateReturnRequestFactory.ThreeMonth(monthYear);
            var threeMonthResult = portfolio.CalculateGrossReturn(threeMonthRequest);
            recordModel.ThreeMonths = threeMonthResult.GetNullValueOnError();

            var twelveMonthRequest = CalculateReturnRequestFactory.TwelveMonth(monthYear);
            var twelveMonthResult = portfolio.CalculateGrossReturn(twelveMonthRequest);
            recordModel.TwelveMonths = twelveMonthResult.GetNullValueOnError();

            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(monthYear);
            var yearToDateResult = portfolio.CalculateGrossReturn(yearToDateRequest);
            recordModel.YearToDate = yearToDateResult.GetNullValueOnError();

            return recordModel;
        }
    }
}