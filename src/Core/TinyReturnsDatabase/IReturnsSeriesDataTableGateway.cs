namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase
{
    public interface IReturnsSeriesDataTableGateway
    {
        int InsertReturnSeries(ReturnSeriesDto returnSeries);

        ReturnSeriesDto GetReturnSeries(int returnSeriesId);

        void DeleteReturnSeries(int returnSeriesId);

        ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers);

        void DeleteAllReturnSeries();
    }
}