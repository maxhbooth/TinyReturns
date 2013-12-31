using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class ReturnsSeriesDataRepositoryDummy : IReturnsSeriesDataRepository
    {
        public virtual int InsertReturnSeries(ReturnSeriesDto returnSeries)
        {
            return 0;
        }

        public virtual ReturnSeriesDto GetReturnSeries(int returnSeriesId)
        {
            return null;
        }

        public virtual void DeleteReturnSeries(int returnSeriesId)
        {
            
        }

        public virtual ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers)
        {
            return null;
        }

        public virtual void DeleteAllReturnSeries()
        {
        }
    }
}