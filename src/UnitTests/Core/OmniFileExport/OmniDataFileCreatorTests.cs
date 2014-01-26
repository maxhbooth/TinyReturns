using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.OmniFileExport;
using Dimensional.TinyReturns.UnitTests.Core.FlatFiles;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core.OmniFileExport
{
    public class OmniDataFileCreatorTests
    {
        private readonly InvestmentVehicleReturnsRepositoryStub _investmentVehicleReturnsRepositoryStub;
        private readonly FlatFileIoSpy _flatFileIoSpy;
        private string _fullFilePath;

        public OmniDataFileCreatorTests()
        {
            _investmentVehicleReturnsRepositoryStub = new InvestmentVehicleReturnsRepositoryStub();
            _flatFileIoSpy = new FlatFileIoSpy();
            _fullFilePath = "c:\\somefolder\\data.dat";
        }

        [Fact]
        public void ShouldWriteHeaderRow()
        {
            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(new MonthYear(2000, 1), _fullFilePath);

            Assert.Equal(1, _flatFileIoSpy.NumberOfLines);
            Assert.Equal("Investment Vehicle Id|Type|Name|Fee Type|Duration|End Date|Return Value", _flatFileIoSpy.FirstLine());
        }

        [Fact]
        public void ShouldHaveCreatedCorrectFile()
        {
            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(new MonthYear(2000, 1), _fullFilePath);

            Assert.Equal(_fullFilePath, _flatFileIoSpy.OpenFileName);
        }

        [Fact]
        public void ShouldWriteSinglePortfolioNetMonthRecord()
        {
            var monthYear = new MonthYear(2000, 1);

            var investmentVehicle = InvestmentVehicleFactoryForTests
                .SetupPortfolio(100, "Portfolio 100")
                .AddNetReturn(monthYear, 0.1m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.Equal(3, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.1"));
        }

        [Fact]
        public void ShouldWriteMonthForYearToCurrentMonth()
        {
            var monthYear = new MonthYear(2000, 3);

            var investmentVehicle = InvestmentVehicleFactoryForTests
                .SetupPortfolio(100, "Portfolio 100")
                .AddNetReturn(monthYear, 0.1m)
                .AddNetReturn(monthYear.AddMonths(-1), 0.2m)
                .AddNetReturn(monthYear.AddMonths(-2), 0.3m)
                .AddNetReturn(monthYear.AddMonths(-3), 0.4m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.Equal(6, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-3-31|0.1"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-2-29|0.2"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.3"));
        }

        [Fact]
        public void ShouldWriteBothNetAndGrossOfFees()
        {
            var monthYear = new MonthYear(2000, 4);

            var investmentVehicle = SetupFourMonthsOfData(monthYear);

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-4-30|0.1"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-3-31|0.2"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-2-29|0.3"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.4"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|M|2000-4-30|0.5"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|M|2000-3-31|0.6"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|M|2000-2-29|0.7"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|M|2000-1-31|0.8"));
        }

        [Fact]
        public void ShouldWriteAddQuarterReturn()
        {
            var monthYear = new MonthYear(2000, 4);

            var investmentVehicle = SetupFourMonthsOfData(monthYear);

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|Q|2000-3-31|1.184"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|Q|2000-3-31|3.896"));
        }

        [Fact]
        public void ShouldWriteAddYearToDateReturn()
        {
            var monthYear = new MonthYear(2000, 4);

            var investmentVehicle = SetupFourMonthsOfData(monthYear);

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|Y|2000-4-30|1.4024"));
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|G|Y|2000-4-30|6.3440"));
        }

        private InvestmentVehicle SetupFourMonthsOfData(
            MonthYear monthYear)
        {
            var investmentVehicle = InvestmentVehicleFactoryForTests
                .SetupPortfolio(100, "Portfolio 100")
                .AddNetReturn(monthYear, 0.1m)
                .AddNetReturn(monthYear.AddMonths(-1), 0.2m)
                .AddNetReturn(monthYear.AddMonths(-2), 0.3m)
                .AddNetReturn(monthYear.AddMonths(-3), 0.4m)
                .AddGrossReturn(monthYear, 0.5m)
                .AddGrossReturn(monthYear.AddMonths(-1), 0.6m)
                .AddGrossReturn(monthYear.AddMonths(-2), 0.7m)
                .AddGrossReturn(monthYear.AddMonths(-3), 0.8m)
                .Create();
            return investmentVehicle;
        }

        [Fact]
        public void ShouldWriteMultiplePortfoliosNetMonthRecord()
        {
            var monthYear = new MonthYear(2000, 1);

            var iv100 = InvestmentVehicleFactoryForTests
                .SetupPortfolio(100, "Portfolio 100")
                .AddNetReturn(monthYear, 0.1m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv100);

            var iv101 = InvestmentVehicleFactoryForTests
                .SetupPortfolio(101, "Portfolio 101")
                .AddNetReturn(monthYear, 0.1m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv101);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.Equal(5, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.1"));
            Assert.True(_flatFileIoSpy.ContainsLine("101|Portfolio|Portfolio 101|N|M|2000-1-31|0.1"));
        }

        [Fact]
        public void ShouldNotWriteBenchmarkData()
        {
            var monthYear = new MonthYear(2000, 1);

            var iv100 = InvestmentVehicleFactoryForTests
                .SetupPortfolio(100, "Portfolio 100")
                .AddNetReturn(monthYear, 0.1m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv100);

            var iv101 = InvestmentVehicleFactoryForTests
                .SetupBenchmark(1000, "Benchmark 101")
                .AddNetReturn(monthYear, 0.1m)
                .Create();

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv101);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear, _fullFilePath);

            Assert.Equal(3, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.1"));
        }

        private OmniDataFileCreator CreateOmniDataFileCreator()
        {
            var omniDataFileCreator = new OmniDataFileCreator(_investmentVehicleReturnsRepositoryStub, _flatFileIoSpy);
            return omniDataFileCreator;
        }
    }
}