namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IMonthlyReturnDataTableGateway
    {
        MonthlyReturnDto[] GetAll();

        void Insert(MonthlyReturnDto[] dtos);
    }
}