using System.Linq;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core.PerformanceReport
{
    public class PerformanceReportExcelCreatorTests
    {
        private readonly PerformanceReportExcelReportViewStub _viewStub;
        private readonly InvestmentVehicleReturnsRepositoryStub _returnsRepository;

        public PerformanceReportExcelCreatorTests()
        {
            _viewStub = new PerformanceReportExcelReportViewStub();
            _returnsRepository = new InvestmentVehicleReturnsRepositoryStub();
        }

        [Fact]
        public void CreateReportShouldPopulateMonthText()
        {
            SetupPortfolioAndBenchmark();

            var performanceReportExcelCreator = CreatePerformanceReportExcelCreator();

            var monthYear = new MonthYear(2000, 4);

            performanceReportExcelCreator.CreateReport(monthYear);

            Assert.Equal("Month: 4/2000", _viewStub.RenderReportModel.MonthText);
        }

        [Fact]
        public void CreateReportShouldSetPropertiesOnPortfolioNetRecords()
        {
            SetupPortfolioAndBenchmark();

            var performanceReportExcelCreator = CreatePerformanceReportExcelCreator();

            var monthYear = new MonthYear(2000, 4);

            performanceReportExcelCreator.CreateReport(monthYear);

            var expectedRecordModel = new PerformanceReportExcelReportRecordModel();
            expectedRecordModel.Name = "Portfolio000";
            expectedRecordModel.Type = "Portfolio";
            expectedRecordModel.FeeType = FeeType.NetOfFees.DisplayName;
            expectedRecordModel.OneMonth = 0.1m;

            var portfolioRecords = _viewStub.RenderReportModel.GetPortfolioFeeRecords(expectedRecordModel.Name, expectedRecordModel.FeeType);

            var portfolioRecord = portfolioRecords.FirstOrDefault();
            Assert.NotNull(portfolioRecord);
            Assert.Equal(expectedRecordModel.Name, portfolioRecord.Name);
            Assert.Equal(expectedRecordModel.Type, portfolioRecord.Type);
            Assert.Equal(expectedRecordModel.FeeType, portfolioRecord.FeeType);
            Assert.Equal(expectedRecordModel.OneMonth, portfolioRecord.OneMonth);
        }

        private void SetupPortfolioAndBenchmark()
        {
            var portfolio000 = CreateTestPortfolio();
            _returnsRepository.AddInvestmentVehicle(portfolio000);

            var benchmark000 = CreateTestBenchmark();
            _returnsRepository.AddInvestmentVehicle(benchmark000);
        }

        private PerformanceReportExcelReportCreator CreatePerformanceReportExcelCreator()
        {
            return new PerformanceReportExcelReportCreator(_returnsRepository, _viewStub);
        }

        private InvestmentVehicle CreateTestPortfolio()
        {
            var portfolio000 = new InvestmentVehicle();

            portfolio000.Name = "Portfolio000";
            portfolio000.InvestmentVehicleType = InvestmentVehicleType.Portfolio;

            var port000NetSeries = new MonthlyReturnSeries();
            port000NetSeries.FeeType = FeeType.NetOfFees;
            port000NetSeries.AddReturn(new MonthYear(2000, 1), 0.01m);
            port000NetSeries.AddReturn(new MonthYear(2000, 2), 0.02m);
            port000NetSeries.AddReturn(new MonthYear(2000, 3), 0.03m);
            port000NetSeries.AddReturn(new MonthYear(2000, 4), 0.04m);
            portfolio000.AddReturnSeries(port000NetSeries);

            var port000GrossSeries = new MonthlyReturnSeries();
            port000GrossSeries.FeeType = FeeType.GrossOfFees;
            port000GrossSeries.AddReturn(new MonthYear(2000, 1), 0.05m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 2), 0.06m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 3), 0.07m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 4), 0.08m);
            portfolio000.AddReturnSeries(port000GrossSeries);

            return portfolio000;
        }

        private InvestmentVehicle CreateTestBenchmark()
        {
            var benchmark000 = new InvestmentVehicle();
            benchmark000.Name = "Benchmark000";
            benchmark000.InvestmentVehicleType = InvestmentVehicleType.Benchmark;

            var bench000Series = new MonthlyReturnSeries();
            bench000Series.FeeType = FeeType.None;
            bench000Series.AddReturn(new MonthYear(2000, 1), 0.011m);
            bench000Series.AddReturn(new MonthYear(2000, 2), 0.012m);
            bench000Series.AddReturn(new MonthYear(2000, 3), 0.013m);
            bench000Series.AddReturn(new MonthYear(2000, 4), 0.014m);
            benchmark000.AddReturnSeries(bench000Series);

            return benchmark000;
        }

        private class PerformanceReportExcelReportViewStub : IPerformanceReportExcelReportView
        {
            private PerformanceReportExcelReportModel _renderReportModel;

            public PerformanceReportExcelReportModel RenderReportModel
            {
                get { return _renderReportModel; }
            }

            public void RenderReport(
                PerformanceReportExcelReportModel model)
            {
                _renderReportModel = model;
            }
        }
    }
}