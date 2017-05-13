namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IBenchmarkToReturnSeriesDataTableGateway
    {
        BenchmarkToReturnSeriesDto[] GetAll();

        void Insert(BenchmarkToReturnSeriesDto[] dtos);
    }
}