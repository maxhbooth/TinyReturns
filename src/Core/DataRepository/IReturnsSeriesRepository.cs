namespace Dimensional.TinyReturns.Core.DataRepository
{
    public interface IReturnsSeriesRepository
    {
        int InsertReturnSeries(ReturnSeriesDto returnSeries);
        ReturnSeriesDto GetReturnSeries(int returnSeriesId);
        void DeleteReturnSeries(int returnSeriesId);
        ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers);

        void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns);
        MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId);
        void DeleteMonthlyReturns(int returnSeriesId);
    }
}