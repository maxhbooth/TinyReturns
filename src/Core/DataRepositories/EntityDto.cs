namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public class EntityDto
    {
        public static EntityDto CreateForPortfolio(
            int number,
            string name)
        {
            return new EntityDto
            {
                EntityNumber = number,
                Name = name,
                EntityTypeCode = EntityType.Portfolio.Code
            };
        }

        public static EntityDto CreateForBenchmark(
            int number,
            string name)
        {
            return new EntityDto
            {
                EntityNumber = number,
                Name = name,
                EntityTypeCode = EntityType.Benchmark.Code
            };
        }

        public int EntityNumber { get; set; }
        public string Name { get; set; }
        public char EntityTypeCode { get; set; }
    }
}