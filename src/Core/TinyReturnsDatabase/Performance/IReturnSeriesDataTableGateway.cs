namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IReturnSeriesDataTableGateway
    {
        ReturnSeriesDto[] GetAll();

        int Inert(ReturnSeriesDto dto);
    }
}