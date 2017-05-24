namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio
{
    public interface IBenchmarkDataTableGateway
    {
        BenchmarkDto[] GetAll();

        void Insert(BenchmarkDto[] dtos);
    }
}