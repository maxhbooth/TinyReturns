using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.FlatFiles;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Dimensional.TinyReturns.Core.PublicWebReport;
using Dimensional.TinyReturns.Core.PublicWebSite;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        public static ISystemLog SystemLog { get; set; }
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { get; set; }
        public static IReturnsSeriesDataTableGateway ReturnsSeriesDataTableGateway { get; set; }

        // **  New One
        public static IReturnSeriesDataTableGateway ReturnSeriesDataTableGateway { get; set; }
        public static IMonthlyReturnDataTableGateway MonthlyReturnDataTableGateway { get; set; }
        public static IPortfolioToReturnSeriesDataTableGateway PortfolioToReturnSeriesDataTableGateway { get; set; }
        public static IMonthlyReturnsDataTableGateway MonthlyReturnsDataTableGateway { get; set; }
        public static ICitiReturnsFileReader CitiReturnsFileReader { get; set; }
        public static IInvestmentVehicleDataTableGateway InvestmentVehicleDataTableGateway { get; set; }
        public static IPerformanceReportExcelReportView PerformanceReportExcelReportView { get; set; }
        public static IFlatFileIo FlatFileIo { get; set; }

        public static IPortfolioDataTableGateway PortfolioDataTableGateway { get; set; }

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
                GetInvestmentVehicleReturnsRepository(),
                FlatFileIo);
        }

        public static InvestmentVehicleReturnsRepository GetInvestmentVehicleReturnsRepository()
        {
            var r = new InvestmentVehicleReturnsRepository(
                InvestmentVehicleDataTableGateway,
                ReturnsSeriesDataTableGateway,
                MonthlyReturnsDataTableGateway);

            return r;
        }

        public static PerformanceReportExcelReportCreator GetPerformanceReportExcelReportCreator()
        {
            return new PerformanceReportExcelReportCreator(
                GetInvestmentVehicleReturnsRepository(),
                PerformanceReportExcelReportView);
        }

        public static PortfolioListPageAdapter GetPortfolioListPageAdapter()
        {
            return new PortfolioListPageAdapter(GetInvestmentVehicleReturnsRepository());
        }

        public static PublicWebReportFacade GetPublicWebReportFacade()
        {
            return new PublicWebReportFacade(PortfolioDataTableGateway);
        }
    }
}