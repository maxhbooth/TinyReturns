namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio
{
    public interface IBenchmarkDataTableGateway
    {
        BenchmarkDto[] GetAll();

        void Insert(BenchmarkDto[] dtos);
    }
}