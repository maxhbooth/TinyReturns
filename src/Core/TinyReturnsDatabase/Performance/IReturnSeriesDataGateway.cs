namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IReturnSeriesDataGateway
    {
        ReturnSeriesDto[] GetAll();

        void Insert(ReturnSeriesDto[] dtos);
    }
}