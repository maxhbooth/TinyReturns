using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.Core.DataRepositories;
using Dimensional.TinyReturns.Core.FlatFiles;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.Core.PublicWebSite;

namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        private static ISystemLog _systemLog;
        public static ISystemLog SystemLog { set { _systemLog = value; } }
        public static ISystemLog GetSystemLog() { return _systemLog; }

        private static ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { set { _tinyReturnsDatabaseSettings = value; } }
        public static ITinyReturnsDatabaseSettings GetTinyReturnsDatabaseSettings() { return _tinyReturnsDatabaseSettings; }

        private static IReturnsSeriesDataRepository _returnsSeriesDataRepository;
        public static IReturnsSeriesDataRepository ReturnsSeriesDataRepository { set { _returnsSeriesDataRepository = value; } }
        public static IReturnsSeriesDataRepository GetReturnsSeriesRepository() { return _returnsSeriesDataRepository; }

        private static IMonthlyReturnsDataRepository _monthlyReturnsDataRepository;
        public static IMonthlyReturnsDataRepository MonthlyReturnsDataRepository { set { _monthlyReturnsDataRepository = value; } }
        public static IMonthlyReturnsDataRepository GetMonthlyReturnsDataRepository() { return _monthlyReturnsDataRepository; }

        private static ICitiReturnsFileReader _citiReturnsFileReader;
        public static ICitiReturnsFileReader CitiReturnsFileReader { set { _citiReturnsFileReader = value; } }
        public static ICitiReturnsFileReader GetCitiReturnsFileReader() { return _citiReturnsFileReader; }

        private static IInvestmentVehicleDataRepository _investmentVehicleDataRepository;
        public static IInvestmentVehicleDataRepository InvestmentVehicleDataRepository { set { _investmentVehicleDataRepository = value; } }
        public static IInvestmentVehicleDataRepository GetInvestmentVehicleDataRepository() { return _investmentVehicleDataRepository; }

        private static IFlatFileIo _flatFileIo;
        public static IFlatFileIo FlatFileIo { set { _flatFileIo = value; } }
        public static IFlatFileIo GetFlatFileIo() { return _flatFileIo; }

        public static CitiReturnSeriesImporter GetCitiReturnSeriesImporter()
        {
            return new CitiReturnSeriesImporter(
                _returnsSeriesDataRepository,
                _citiReturnsFileReader,
                _monthlyReturnsDataRepository);
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

        public static PortfolioListPageAdapter GetPortfolioListPageAdapter()
        {
            return new PortfolioListPageAdapter(GetInvestmentVehicleReturnsRepository());
        }
    }
}