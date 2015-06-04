using System.Collections.Generic;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class InvestmentVehicleReturnsRepositoryStub : IInvestmentVehicleReturnsRepository
    {
        private readonly List<InvestmentVehicle> _investmentVehicleList;

        public InvestmentVehicleReturnsRepositoryStub()
        {
            _investmentVehicleList = new List<InvestmentVehicle>();
        }

        public void AddInvestmentVehicle(
            InvestmentVehicle i)
        {
            _investmentVehicleList.Add(i);
        }

        public InvestmentVehicle[] GetAllInvestmentVehicles()
        {
            return _investmentVehicleList.ToArray();
        }
    }
}