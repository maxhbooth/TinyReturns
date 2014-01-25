using System.Linq;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class InvestmentVehicleFactoryForTests
    {
        public static InvestmentVehicleSetupForTests SetupPortfolio(
            int number, string name)
        {
            var portfolio = InvestmentVehicle.CreatePortfolio(number, name);

            return new InvestmentVehicleSetupForTests(portfolio);
        }

        public static InvestmentVehicleSetupForTests SetupBenchmark(
            int number, string name)
        {
            var portfolio = InvestmentVehicle.CreateBenchmark(number, name);

            return new InvestmentVehicleSetupForTests(portfolio);
        }

        public class InvestmentVehicleSetupForTests
        {
            private readonly InvestmentVehicle _investmentVehicle;

            public InvestmentVehicleSetupForTests(
                InvestmentVehicle investmentVehicle)
            {
                _investmentVehicle = investmentVehicle;
            }

            public InvestmentVehicleSetupForTests AddNetReturn(
                MonthYear monthYear,
                decimal returnValue)
            {
                return AddReturn(monthYear, returnValue, FeeType.NetOfFees);
            }

            public InvestmentVehicleSetupForTests AddGrossReturn(
                MonthYear monthYear,
                decimal returnValue)
            {
                return AddReturn(monthYear, returnValue, FeeType.GrossOfFees);
            }

            private InvestmentVehicleSetupForTests AddReturn(
                MonthYear monthYear,
                decimal returnValue,
                FeeType feeType)
            {
                var netSeries = _investmentVehicle.GetAllReturnSeries().FirstOrDefault(s => { return s.FeeType == feeType; });

                if (netSeries == null)
                {
                    netSeries = new MonthlyReturnSeries() {FeeType = feeType};
                    _investmentVehicle.AddReturnSeries(netSeries);
                }

                netSeries.AddReturn(monthYear, returnValue);

                return this;
            }

            public InvestmentVehicle Create()
            {
                return _investmentVehicle;
            }
        }
    }
}