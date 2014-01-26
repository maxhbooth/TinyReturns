using System.Collections.Generic;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class InvestmentVehicleReturnsRepositoryStub : InvestmentVehicleReturnsRepository
    {
        private readonly List<InvestmentVehicle> _investmentVehicleList;

        public InvestmentVehicleReturnsRepositoryStub()
            : base(null, null, null)
        {
            _investmentVehicleList = new List<InvestmentVehicle>();
        }

        public void AddInvestmentVehicle(
            InvestmentVehicle i)
        {
            _investmentVehicleList.Add(i);
        }

        public override InvestmentVehicle[] GetAllInvestmentVehicles()
        {
            return _investmentVehicleList.ToArray();
        }
    }
}