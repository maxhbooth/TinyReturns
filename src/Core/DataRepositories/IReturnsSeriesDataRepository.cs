namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public interface IReturnsSeriesDataRepository
    {
        int InsertReturnSeries(ReturnSeriesDto returnSeries);
        ReturnSeriesDto GetReturnSeries(int returnSeriesId);
        void DeleteReturnSeries(int returnSeriesId);
        ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers);
        void DeleteAllReturnSeries();
    }
}