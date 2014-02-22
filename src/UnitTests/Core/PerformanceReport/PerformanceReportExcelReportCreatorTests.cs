using System.Linq;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core.PerformanceReport
{
    public class PerformanceReportExcelReportCreatorTests
    {
        private readonly PerformanceReportExcelReportViewStub _viewStub;
        private readonly InvestmentVehicleReturnsRepositoryStub _returnsRepository;

        public PerformanceReportExcelReportCreatorTests()
        {
            _viewStub = new PerformanceReportExcelReportViewStub();
            _returnsRepository = new InvestmentVehicleReturnsRepositoryStub();
        }

        [Fact]
        public void CreateReportShouldPopulateMonthText()
        {
            var monthYear = new MonthYear(2000, 4);

            SetupPortfolioAndBenchmark();

            var performanceReportExcelCreator = CreatePerformanceReportExcelCreator();

            performanceReportExcelCreator.CreateReport(monthYear);

            Assert.Equal("Month: 4/2000", _viewStub.RenderReportModel.MonthText);
        }

        [Fact]
        public void CreateReportShouldSetPropertiesOnPortfolioNetRecordsWhenAllDataIsPopulated()
        {
            var monthYear = new MonthYear(2000, 4);

            SetupPortfolioAndBenchmark();

            var performanceReportExcelCreator = CreatePerformanceReportExcelCreator();

            performanceReportExcelCreator.CreateReport(monthYear);

            var expectedRecordModel = new PerformanceReportExcelReportRecordModel();
            expectedRecordModel.EntityNumber = 100;
            expectedRecordModel.Name = "Portfolio000";
            expectedRecordModel.Type = "Portfolio";
            expectedRecordModel.FeeType = FeeType.NetOfFees.DisplayName;
            expectedRecordModel.OneMonth = 0.04m;
            expectedRecordModel.ThreeMonths = 0.092624m;
            expectedRecordModel.TwelveMonths = -0.238239267167793646673920m;
            expectedRecordModel.YearToDate = 0.10355024m;

            var portfolioRecords = _viewStub.RenderReportModel.GetPortfolioFeeRecords(expectedRecordModel.Name, expectedRecordModel.FeeType);

            var portfolioRecord = portfolioRecords.FirstOrDefault();
            Assert.NotNull(portfolioRecord);

            AssertRecordIsEqual(expectedRecordModel, portfolioRecord);
        }

        [Fact]
        public void CreateReportShouldSetPropertiesOnPortfolioNetRecordsWhenAllCalculationsHaveErrored()
        {
            var monthYear = new MonthYear(2000, 4);

            SetupPortfolioAndBenchmark(monthYear, monthYear);

            var performanceReportExcelCreator = CreatePerformanceReportExcelCreator();

            performanceReportExcelCreator.CreateReport(monthYear);

            var expectedRecordModel = new PerformanceReportExcelReportRecordModel();
            expectedRecordModel.EntityNumber = 100;
            expectedRecordModel.Name = "Portfolio000";
            expectedRecordModel.Type = "Portfolio";
            expectedRecordModel.FeeType = FeeType.NetOfFees.DisplayName;
            expectedRecordModel.OneMonth = null;
            expectedRecordModel.ThreeMonths = null;
            expectedRecordModel.TwelveMonths = null;
            expectedRecordModel.YearToDate = null;

            var portfolioRecords = _viewStub.RenderReportModel.GetPortfolioFeeRecords(expectedRecordModel.Name, expectedRecordModel.FeeType);

            var portfolioRecord = portfolioRecords.FirstOrDefault();
            Assert.NotNull(portfolioRecord);

            AssertRecordIsEqual(expectedRecordModel, portfolioRecord);
        }

        private void AssertRecordIsEqual(
            PerformanceReportExcelReportRecordModel expectedRecordModel,
            PerformanceReportExcelReportRecordModel portfolioRecord)
        {
            Assert.Equal(expectedRecordModel.EntityNumber, portfolioRecord.EntityNumber);
            Assert.Equal(expectedRecordModel.Name, portfolioRecord.Name);
            Assert.Equal(expectedRecordModel.Type, portfolioRecord.Type);
            Assert.Equal(expectedRecordModel.FeeType, portfolioRecord.FeeType);
            Assert.Equal(expectedRecordModel.OneMonth, portfolioRecord.OneMonth);
            Assert.Equal(expectedRecordModel.ThreeMonths, portfolioRecord.ThreeMonths);
            Assert.Equal(expectedRecordModel.TwelveMonths, portfolioRecord.TwelveMonths);
            Assert.Equal(expectedRecordModel.YearToDate, portfolioRecord.YearToDate);
        }

        private void SetupPortfolioAndBenchmark()
        {
            SetupPortfolioAndBenchmark(null, null);
        }

        private void SetupPortfolioAndBenchmark(
            MonthYear portfolioExcludeMonth,
            MonthYear benchmarkExcludeMonth)
        {
            var portfolio000 = CreateTestPortfolio(portfolioExcludeMonth);
            _returnsRepository.AddInvestmentVehicle(portfolio000);

            var benchmark000 = CreateTestBenchmark(benchmarkExcludeMonth);
            _returnsRepository.AddInvestmentVehicle(benchmark000);
        }
        
        private PerformanceReportExcelReportCreator CreatePerformanceReportExcelCreator()
        {
            return new PerformanceReportExcelReportCreator(_returnsRepository, _viewStub);
        }

        private InvestmentVehicle CreateTestPortfolio(
            MonthYear excludeMonth = null)
        {
            var portfolio000 = new InvestmentVehicle();

            portfolio000.Number = 100;
            portfolio000.Name = "Portfolio000";
            portfolio000.InvestmentVehicleType = InvestmentVehicleType.Portfolio;

            var port000NetSeries = new MonthlyReturnSeries();
            port000NetSeries.FeeType = FeeType.NetOfFees;
            port000NetSeries.AddReturn(new MonthYear(1999, 5), -0.08m);
            port000NetSeries.AddReturn(new MonthYear(1999, 6), -0.07m);
            port000NetSeries.AddReturn(new MonthYear(1999, 7), -0.06m);
            port000NetSeries.AddReturn(new MonthYear(1999, 8), -0.05m);
            port000NetSeries.AddReturn(new MonthYear(1999, 9), -0.04m);
            port000NetSeries.AddReturn(new MonthYear(1999, 10), -0.03m);
            port000NetSeries.AddReturn(new MonthYear(1999, 11), -0.02m);
            port000NetSeries.AddReturn(new MonthYear(1999, 12), -0.01m);
            port000NetSeries.AddReturn(new MonthYear(2000, 1), 0.01m);
            port000NetSeries.AddReturn(new MonthYear(2000, 2), 0.02m);
            port000NetSeries.AddReturn(new MonthYear(2000, 3), 0.03m);
            port000NetSeries.AddReturn(new MonthYear(2000, 4), 0.04m);

            if (excludeMonth != null)
                port000NetSeries.RemoveReturn(excludeMonth);

            portfolio000.AddReturnSeries(port000NetSeries);

            var port000GrossSeries = new MonthlyReturnSeries();
            port000GrossSeries.FeeType = FeeType.GrossOfFees;
            port000GrossSeries.AddReturn(new MonthYear(1999, 5), -0.04m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 6), -0.03m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 7), -0.02m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 8), -0.01m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 9), 0.01m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 10), 0.02m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 11), 0.03m);
            port000GrossSeries.AddReturn(new MonthYear(1999, 12), 0.04m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 1), 0.05m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 2), 0.06m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 3), 0.07m);
            port000GrossSeries.AddReturn(new MonthYear(2000, 4), 0.08m);

            if (excludeMonth != null)
                port000GrossSeries.RemoveReturn(excludeMonth);

            portfolio000.AddReturnSeries(port000GrossSeries);

            return portfolio000;
        }

        private InvestmentVehicle CreateTestBenchmark(
            MonthYear excludeMonth = null)
        {
            var benchmark000 = new InvestmentVehicle();
            benchmark000.Number = 10000;
            benchmark000.Name = "Benchmark000";
            benchmark000.InvestmentVehicleType = InvestmentVehicleType.Benchmark;

            var bench000Series = new MonthlyReturnSeries();
            bench000Series.FeeType = FeeType.None;
            bench000Series.AddReturn(new MonthYear(1999, 5), 0.002m);
            bench000Series.AddReturn(new MonthYear(1999, 6), 0.003m);
            bench000Series.AddReturn(new MonthYear(1999, 7), 0.005m);
            bench000Series.AddReturn(new MonthYear(1999, 8), 0.006m);
            bench000Series.AddReturn(new MonthYear(1999, 9), 0.007m);
            bench000Series.AddReturn(new MonthYear(1999, 10), 0.008m);
            bench000Series.AddReturn(new MonthYear(1999, 11), 0.009m);
            bench000Series.AddReturn(new MonthYear(1999, 12), 0.010m);
            bench000Series.AddReturn(new MonthYear(2000, 1), 0.011m);
            bench000Series.AddReturn(new MonthYear(2000, 2), 0.012m);
            bench000Series.AddReturn(new MonthYear(2000, 3), 0.013m);
            bench000Series.AddReturn(new MonthYear(2000, 4), 0.014m);

            if (excludeMonth != null)
                bench000Series.RemoveReturn(excludeMonth);

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