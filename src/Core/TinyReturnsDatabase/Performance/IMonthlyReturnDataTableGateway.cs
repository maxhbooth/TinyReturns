namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IMonthlyReturnDataTableGateway
    {
        MonthlyReturnDto[] GetAll();

        MonthlyReturnDto[] Get(
            int[] returnSeriesIds);

        void Insert(MonthlyReturnDto[] dtos);
    }
}