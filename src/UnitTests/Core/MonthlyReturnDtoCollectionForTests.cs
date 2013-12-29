using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    //MonthlyReturnDto
    public class MonthlyReturnDtoCollectionForTests
    {
        private readonly List<MonthlyReturnDto> _monthlyReturnsList;

        public MonthlyReturnDtoCollectionForTests()
        {
            _monthlyReturnsList = new List<MonthlyReturnDto>();
        }

        public MonthlyReturnDtoCollectionForTests Add(
            MonthlyReturnDto d)
        {
            _monthlyReturnsList.Add(d);

            return this;
        }

        public MonthlyReturnDto[] GetReturnSeriesDtos()
        {
            return _monthlyReturnsList.ToArray();
        }
    }
}