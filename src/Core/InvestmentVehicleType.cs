using System.Linq;
using Headspring;

namespace Dimensional.TinyReturns.Core
{
    public class InvestmentVehicleType : Enumeration<InvestmentVehicleType>
    {
        public static InvestmentVehicleType Portfolio = new InvestmentVehicleType(1, "Portfolio", 'P');
        public static InvestmentVehicleType Benchmark = new InvestmentVehicleType(2, "Benchmark", 'B');

        private readonly char _code;

        public InvestmentVehicleType(
            int value,
            string displayName,
            char code)
            : base(value, displayName)
        {
            _code = code;
        }

        public char Code
        {
            get { return _code; }
        }

        public static InvestmentVehicleType FromCode(char code)
        {
            var entityTypes = GetAll();

            return entityTypes.FirstOrDefault(e => e.Code == code);
        }
    }
}