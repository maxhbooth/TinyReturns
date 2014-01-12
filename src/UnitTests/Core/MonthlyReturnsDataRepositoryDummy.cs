using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class MonthlyReturnsDataRepositoryDummy : IMonthlyReturnsDataRepository
    {
        public virtual void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns)
        {
        }

        public virtual MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId)
        {
            return null;
        }

        public virtual MonthlyReturnDto[] GetMonthlyReturns(int[] returnSeriesIds)
        {
            return null;
        }

        public virtual void DeleteMonthlyReturns(int returnSeriesId)
        {
        }

        public virtual void DeleteAllMonthlyReturns()
        {
        }
         
    }
}