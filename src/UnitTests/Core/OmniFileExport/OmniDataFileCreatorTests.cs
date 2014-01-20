using System.Collections.Generic;
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

        public OmniDataFileCreatorTests()
        {
            _investmentVehicleReturnsRepositoryStub = new InvestmentVehicleReturnsRepositoryStub();
            _flatFileIoSpy = new FlatFileIoSpy();
        }

        [Fact]
        public void ShouldWriteHeaderRow()
        {
            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(new MonthYear(2000, 1));

            Assert.Equal(1, _flatFileIoSpy.NumberOfLines);
            Assert.Equal("Investment Vehicle Id|Type|Name|Fee Type|Duration|End Date|Return Value", _flatFileIoSpy.FirstLine());
        }

        [Fact]
        public void ShouldWriteSinglePortfolioNetMonthRecord()
        {
            var investmentVehicle = InvestmentVehicle.CreatePortfolio(100, "Portfolio 100");

            var netReturnSeries = new MonthlyReturnSeries();
            netReturnSeries.FeeType = FeeType.NetOfFees;

            var monthYear = new MonthYear(2000, 1);

            netReturnSeries.AddReturn(monthYear, 0.1m);

            investmentVehicle.AddReturnSeries(netReturnSeries);

            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(investmentVehicle);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear);

            Assert.Equal(2, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.1"));
        }

        [Fact]
        public void ShouldWriteMultiplePortfoliosNetMonthRecord()
        {
            var monthYear = new MonthYear(2000, 1);

            var iv100 = InvestmentVehicle.CreatePortfolio(100, "Portfolio 100");
            AddSingleMonthlyReturn(monthYear, iv100);
            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv100);

            var iv101 = InvestmentVehicle.CreatePortfolio(101, "Portfolio 101");
            AddSingleMonthlyReturn(monthYear, iv101);
            _investmentVehicleReturnsRepositoryStub.AddInvestmentVehicle(iv101);

            var omniDataFileCreator = CreateOmniDataFileCreator();
            omniDataFileCreator.CreateFile(monthYear);

            Assert.Equal(3, _flatFileIoSpy.NumberOfLines);
            Assert.True(_flatFileIoSpy.ContainsLine("100|Portfolio|Portfolio 100|N|M|2000-1-31|0.1"));
            Assert.True(_flatFileIoSpy.ContainsLine("101|Portfolio|Portfolio 101|N|M|2000-1-31|0.1"));
        }

        private void AddSingleMonthlyReturn(
            MonthYear monthYear,
            InvestmentVehicle iv100)
        {
            var netReturnSeries = new MonthlyReturnSeries();
            netReturnSeries.FeeType = FeeType.NetOfFees;
            netReturnSeries.AddReturn(monthYear, 0.1m);
            iv100.AddReturnSeries(netReturnSeries);
        }

        private OmniDataFileCreator CreateOmniDataFileCreator()
        {
            var omniDataFileCreator = new OmniDataFileCreator(_investmentVehicleReturnsRepositoryStub, _flatFileIoSpy);
            return omniDataFileCreator;
        }

        public class InvestmentVehicleReturnsRepositoryStub : InvestmentVehicleReturnsRepository
        {
            private readonly List<InvestmentVehicle> _investmentVehicleList;

            public InvestmentVehicleReturnsRepositoryStub(): base(null, null, null)
            {
                _investmentVehicleList = new List<InvestmentVehicle>();
            }

            public void AddInvestmentVehicle(
                InvestmentVehicle i)
            {
                _investmentVehicleList.Add(i);
            }

            public override InvestmentVehicle[] GetEntitiesWithReturnSeries()
            {
                return _investmentVehicleList.ToArray();
            }
        }
    }
}