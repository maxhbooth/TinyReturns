namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase
{
    public interface IReturnsSeriesDataGateway
    {
        int InsertReturnSeries(ReturnSeriesDto returnSeries);

        ReturnSeriesDto GetReturnSeries(int returnSeriesId);

        void DeleteReturnSeries(int returnSeriesId);

        ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers);

        void DeleteAllReturnSeries();
    }
}