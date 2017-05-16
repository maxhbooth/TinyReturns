namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance
{
    public interface IMonthlyReturnDataTableGateway
    {
        MonthlyReturnDto[] GetAll();

        MonthlyReturnDto[] Get(
            int[] returnSeriesIds);

        void Insert(MonthlyReturnDto[] dtos);
    }
}