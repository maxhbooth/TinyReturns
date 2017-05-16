namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance
{
    public interface IBenchmarkToReturnSeriesDataTableGateway
    {
        BenchmarkToReturnSeriesDto[] GetAll();

        void Insert(BenchmarkToReturnSeriesDto[] dtos);
    }
}