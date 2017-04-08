namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase
{
    public interface IMonthlyReturnsDataTableGateway
    {
        void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns);

        MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId);

        MonthlyReturnDto[] GetMonthlyReturns(int[] returnSeriesIds);

        void DeleteMonthlyReturns(int returnSeriesId);

        void DeleteAllMonthlyReturns();
    }
}