namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IReturnSeriesDataTableGateway
    {
        ReturnSeriesDto[] GetAll();

        int Insert(ReturnSeriesDto dto);

        void Insert(ReturnSeriesDto[] dtos);
    }
}