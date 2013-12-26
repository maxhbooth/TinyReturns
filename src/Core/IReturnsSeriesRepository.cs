namespace Dimensional.TinyReturns.Core
{
    public interface IReturnsSeriesRepository
    {
        int InsertReturnSeries(ReturnSeries returnSeries);
        ReturnSeries GetReturnSeries(int returnSeriesId);
        void DeleteReturnSeries(int returnSeriesId);

        void InsertMonthlyReturns(MonthlyReturn[] monthlyReturns);
        MonthlyReturn[] GetMonthlyReturns(int returnSeriesId);
        void DeleteMonthlyReturns(int returnSeriesId);
    }
}