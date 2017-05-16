using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.OmniFileExport;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Dimensional.TinyReturns.Core.ReturnSeriesImportContext.Services.CitiFileImport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        public static ISystemLog SystemLog { get; set; }
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { get; set; }
        public static IReturnSeriesDataTableGateway ReturnSeriesDataTableGateway { get; set; }
        public static IMonthlyReturnDataTableGateway MonthlyReturnDataTableGateway { get; set; }
        public static IPortfolioToReturnSeriesDataTableGateway PortfolioToReturnSeriesDataTableGateway { get; set; }
        public static ICitiReturnsFileReader CitiReturnsFileReader { get; set; }
        public static IPerformanceReportExcelReportView PerformanceReportExcelReportView { get; set; }
        public static IFlatFileIo FlatFileIo { get; set; }

        public static IPortfolioDataTableGateway PortfolioDataTableGateway { get; set; }
        public static IBenchmarkDataTableGateway BenchmarkDataTableGateway { get; set; }
        public static IBenchmarkToReturnSeriesDataTableGateway BenchmarkToReturnSeriesDataTableGateway { get; set; }
        public static IPortfolioToBenchmarkDataTableGateway PortfolioToBenchmarkDataTableGateway { get; set; }

        // **

        public static CitiMonthyReturnImporter GetCitiReturnSeriesImporter()
        {
            return new CitiMonthyReturnImporter(
                CitiReturnsFileReader,
                ReturnSeriesDataTableGateway,
                MonthlyReturnDataTableGateway,
                PortfolioToReturnSeriesDataTableGateway);
        }

        public static OmniDataFilePresenter GetOmniDataFileCreator()
        {
            var omniDataFileView = new OmniDataFileView(
                FlatFileIo);

            return new OmniDataFilePresenter(
                CreatePortfolioWithPerformanceRepository(),
                omniDataFileView);
        }

        public static PerformanceReportExcelReportPresenter GetPerformanceReportExcelReportCreator()
        {
            return new PerformanceReportExcelReportPresenter(
                CreatePortfolioWithPerformanceRepository(),
                PerformanceReportExcelReportView);
        }

        public static PublicWebReportFacade GetPublicWebReportFacade()
        {
            var portfolioWithPerformanceRepository = CreatePortfolioWithPerformanceRepository();

            return new PublicWebReportFacade(
                portfolioWithPerformanceRepository,
                new Clock());
        }

        private static PortfolioWithPerformanceRepository CreatePortfolioWithPerformanceRepository()
        {
            var returnSeriesRepository = new ReturnSeriesRepository(
                ReturnSeriesDataTableGateway,
                MonthlyReturnDataTableGateway);

            var benchmarkWithPerformanceRepository = new BenchmarkWithPerformanceRepository(
                BenchmarkDataTableGateway,
                BenchmarkToReturnSeriesDataTableGateway,
                returnSeriesRepository);

            return new PortfolioWithPerformanceRepository(
                PortfolioDataTableGateway,
                PortfolioToReturnSeriesDataTableGateway,
                PortfolioToBenchmarkDataTableGateway,
                returnSeriesRepository,
                benchmarkWithPerformanceRepository);
        }
    }
}