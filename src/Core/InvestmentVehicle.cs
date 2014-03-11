using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class InvestmentVehicle
    {
        private readonly List<MonthlyReturnSeries> _returnSeries;

        public static InvestmentVehicle CreatePortfolio(
            int number, string name)
        {
            return new InvestmentVehicle()
            {
                Number = number,
                Name = name,
                InvestmentVehicleType = InvestmentVehicleType.Portfolio
            };
        }

        public static InvestmentVehicle CreateBenchmark(
            int number, string name)
        {
            return new InvestmentVehicle()
            {
                Number = number,
                Name = name,
                InvestmentVehicleType = InvestmentVehicleType.Benchmark
            };
        }

        public InvestmentVehicle()
        {
            _returnSeries = new List<MonthlyReturnSeries>();
        }

        public int Number { get; set; }
        public string Name { get; set; }
        public InvestmentVehicleType InvestmentVehicleType { get; set; }

        public void AddReturnSeries(
            IEnumerable<MonthlyReturnSeries> a)
        {
            _returnSeries.AddRange(a);
        }

        public void AddReturnSeries(
            MonthlyReturnSeries a)
        {
            _returnSeries.Add(a);
        }

        public MonthlyReturn[] GetNetReturnsInRange(
            MonthYearRange monthRange)
        {
            return GetReturnsInRange(monthRange, FeeType.NetOfFees);
        }

        public MonthlyReturn[] GetGrossReturnsInRange(
            MonthYearRange monthRange)
        {
            return GetReturnsInRange(monthRange, FeeType.GrossOfFees);
        }

        public MonthlyReturn[] GetReturnsInRange(
            MonthYearRange monthRange,
            FeeType feeType)
        {
            var netReturnSeries = GetReturnSeries(feeType);

            if (netReturnSeries == null)
                return new MonthlyReturn[0];

            return netReturnSeries.GetReturnsInRange(monthRange);
        }

        public MonthlyReturnSeries GetReturnSeries(FeeType feeType)
        {
            return _returnSeries
                .FirstOrDefault(s => { return s.FeeType == feeType; });
        }

        public ReturnResult CalculateReturn(
            CalculateReturnRequest request,
            FeeType feeType)
        {
            var netSeries = GetReturnSeries(feeType);

            if (netSeries == null)
            {
                var errorMessage = string.Format("Could not find '{0}' return series.", feeType.DisplayName);

                return ReturnResult.CreateWithError(errorMessage);
            }

            return netSeries.CalculateReturn(request);
        }

        public MonthlyReturnSeries[] GetAllReturnSeries()
        {
            return _returnSeries.ToArray();
        }

        public int ReturnSeriesCount
        {
            get { return _returnSeries.Count; }
        }

        // ** Equality

        protected bool Equals(InvestmentVehicle other)
        {
            if (!_returnSeries.SequenceEqual(other._returnSeries))
                return false;

            return Number == other.Number && string.Equals(Name, other.Name) && Equals(InvestmentVehicleType, other.InvestmentVehicleType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InvestmentVehicle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Number;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InvestmentVehicleType != null ? InvestmentVehicleType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(InvestmentVehicle left, InvestmentVehicle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InvestmentVehicle left, InvestmentVehicle right)
        {
            return !Equals(left, right);
        }
    }

    public static class InvestmentVehicleExtensions
    {
        public static IEnumerable<InvestmentVehicle> GetPortfolios(
            this IEnumerable<InvestmentVehicle> investmentVehicles)
        {
            return investmentVehicles.Where(i => i.InvestmentVehicleType == InvestmentVehicleType.Portfolio);
        }

        public static IEnumerable<InvestmentVehicle> GetBenchmarks(
            this IEnumerable<InvestmentVehicle> investmentVehicles)
        {
            return investmentVehicles.Where(i => i.InvestmentVehicleType == InvestmentVehicleType.Benchmark);
        }
    }
}