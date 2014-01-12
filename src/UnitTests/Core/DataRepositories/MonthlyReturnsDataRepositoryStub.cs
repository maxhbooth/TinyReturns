using System;
using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core.DataRepositories
{
    public class MonthlyReturnsDataRepositoryStub : MonthlyReturnsDataRepositoryDummy
    {
        private readonly Dictionary<int[], MonthlyReturnDto[]> _getMonthlyReturnsSetups;

        public MonthlyReturnsDataRepositoryStub()
        {
            _getMonthlyReturnsSetups = new Dictionary<int[], MonthlyReturnDto[]>(new IntArrayEqualityComparer());
        }

        public void SetupGetMonthlyReturns(
            int[] returnSeriesIds,
            Action<MonthlyReturnDtoCollectionForTests> listAction)
        {
            var col = new MonthlyReturnDtoCollectionForTests();

            listAction(col);

            _getMonthlyReturnsSetups.Add(returnSeriesIds, col.GetReturnSeriesDtos());
        }

        public override MonthlyReturnDto[] GetMonthlyReturns(int[] returnSeriesIds)
        {
            if (_getMonthlyReturnsSetups.ContainsKey(returnSeriesIds))
                return _getMonthlyReturnsSetups[returnSeriesIds];

            return new MonthlyReturnDto[0];
        }

    }
}