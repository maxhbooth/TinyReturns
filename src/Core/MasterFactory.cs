using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.FlatFiles;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.Core.PerformanceReport;
using Dimensional.TinyReturns.Core.PublicWebSite;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        private static ISystemLog _systemLog;
        public static ISystemLog SystemLog { set { _systemLog = value; } }
        public static ISystemLog GetSystemLog() { return _systemLog; }

        // **

        private static ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { set { _tinyReturnsDatabaseSettings = value; } }
        public static ITinyReturnsDatabaseSettings GetTinyReturnsDatabaseSettings() { return _tinyReturnsDatabaseSettings; }

        // **

        private static IReturnsSeriesDataTableGateway _returnsSeriesDataTableGateway;
        public static IReturnsSeriesDataTableGateway ReturnsSeriesDataTableGateway { set { _returnsSeriesDataTableGateway = value; } }
        public static IReturnsSeriesDataTableGateway GetReturnsSeriesRepository() { return _returnsSeriesDataTableGateway; }

        // **

        private static IMonthlyReturnsDataTableGateway _monthlyReturnsDataTableGateway;
        public static IMonthlyReturnsDataTableGateway MonthlyReturnsDataTableGateway { set { _monthlyReturnsDataTableGateway = value; } }
        public static IMonthlyReturnsDataTableGateway GetMonthlyReturnsDataRepository() { return _monthlyReturnsDataTableGateway; }

        // **

        private static ICitiReturnsFileReader _citiReturnsFileReader;
        public static ICitiReturnsFileReader CitiReturnsFileReader { set { _citiReturnsFileReader = value; } }
        public static ICitiReturnsFileReader GetCitiReturnsFileReader() { return _citiReturnsFileReader; }

        // **

        private static IInvestmentVehicleDataTableGateway _investmentVehicleDataTableGateway;
        public static IInvestmentVehicleDataTableGateway InvestmentVehicleDataTableGateway { set { _investmentVehicleDataTableGateway = value; } }
        public static IInvestmentVehicleDataTableGateway GetInvestmentVehicleDataRepository() { return _investmentVehicleDataTableGateway; }

        // **

        private static IPerformanceReportExcelReportView _performanceReportExcelReportView;
        public static IPerformanceReportExcelReportView PerformanceReportExcelReportView { set { _performanceReportExcelReportView = value; } }
        public static IPerformanceReportExcelReportView GetPerformanceReportExcelReportView() { return _performanceReportExcelReportView; }

        // **

        private static IFlatFileIo _flatFileIo;
        public static IFlatFileIo FlatFileIo { set { _flatFileIo = value; } }
        public static IFlatFileIo GetFlatFileIo() { return _flatFileIo; }

        // **

        public static ICitiFileImportInteractor GetCitiFileImportInteractor()
        {
            return new CitiFileImportInteractor(GetCitiReturnSeriesImporter());
        }
        
        public static CitiReturnSeriesImporter GetCitiReturnSeriesImporter()
        {
            return new CitiReturnSeriesImporter(
                _returnsSeriesDataTableGateway,
                _citiReturnsFileReader,
                _monthlyReturnsDataTableGateway);
        }

        public static OmniDataFileCreator GetOmniDataFileCreator()
        {
            return new OmniDataFileCreator(
                GetInvestmentVehicleReturnsRepository(),
                GetFlatFileIo());
        }

        public static InvestmentVehicleReturnsRepository GetInvestmentVehicleReturnsRepository()
        {
            var r = new InvestmentVehicleReturnsRepository(
                GetInvestmentVehicleDataRepository(),
                GetReturnsSeriesRepository(),
                GetMonthlyReturnsDataRepository());

            return r;
        }

        public static PerformanceReportExcelReportCreator GetPerformanceReportExcelReportCreator()
        {
            return new PerformanceReportExcelReportCreator(
                GetInvestmentVehicleReturnsRepository(),
                GetPerformanceReportExcelReportView());
        }

        public static PortfolioListPageAdapter GetPortfolioListPageAdapter()
        {
            return new PortfolioListPageAdapter(GetInvestmentVehicleReturnsRepository());
        }
    }
}