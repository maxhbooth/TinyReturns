namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance
{
    public interface IReturnSeriesDataTableGateway
    {
        ReturnSeriesDto[] GetAll();

        ReturnSeriesDto[] Get(int[] returnSeriesIds);

        int Insert(ReturnSeriesDto dto);

        void Insert(ReturnSeriesDto[] dtos);
    }
}