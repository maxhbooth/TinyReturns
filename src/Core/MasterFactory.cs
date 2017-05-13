﻿using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.FlatFiles;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

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

        // **

        public static CitiMonthyReturnImporter GetCitiReturnSeriesImporter()
        {
            return new CitiMonthyReturnImporter(
                CitiReturnsFileReader,
                ReturnSeriesDataTableGateway,
                MonthlyReturnDataTableGateway,
                PortfolioToReturnSeriesDataTableGateway);
        }

        public static OmniDataFileCreator GetOmniDataFileCreator()
        {
            return new OmniDataFileCreator(
                CreatePortfolioWithPerformanceRepository(),
                FlatFileIo);
        }

        public static PerformanceReportExcelReportProjector GetPerformanceReportExcelReportCreator()
        {
            return new PerformanceReportExcelReportProjector(
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

            return new PortfolioWithPerformanceRepository(
                PortfolioDataTableGateway,
                PortfolioToReturnSeriesDataTableGateway,
                returnSeriesRepository);
        }
    }
}