namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public class InvestmentVehicleDto
    {
        public static InvestmentVehicleDto CreateForPortfolio(
            int number,
            string name)
        {
            return new InvestmentVehicleDto
            {
                InvestmentVehicleNumber = number,
                Name = name,
                InvestmentVehicleTypeCode = InvestmentVehicleType.Portfolio.Code
            };
        }

        public static InvestmentVehicleDto CreateForBenchmark(
            int number,
            string name)
        {
            return new InvestmentVehicleDto
            {
                InvestmentVehicleNumber = number,
                Name = name,
                InvestmentVehicleTypeCode = InvestmentVehicleType.Benchmark.Code
            };
        }

        public int InvestmentVehicleNumber { get; set; }
        public string Name { get; set; }
        public char InvestmentVehicleTypeCode { get; set; }
    }
}