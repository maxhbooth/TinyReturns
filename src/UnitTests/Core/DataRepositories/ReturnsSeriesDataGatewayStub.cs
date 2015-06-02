using System;
using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core.DataRepositories
{
    public class ReturnsSeriesDataGatewayStub : ReturnsSeriesDataGatewayDummy
    {
        private readonly Dictionary<int[], ReturnSeriesDto[]> _getReturnSeriesSetups;

        public ReturnsSeriesDataGatewayStub()
        {
            _getReturnSeriesSetups = new Dictionary<int[], ReturnSeriesDto[]>(new IntArrayEqualityComparer());
        }

        public void SetupGetReturnSeries(
            int[] entityNumbers,
            Action<ReturnSeriesDtoCollectionForTests> listAction)
        {
            var col = new ReturnSeriesDtoCollectionForTests();

            listAction(col);

            _getReturnSeriesSetups.Add(entityNumbers, col.GetReturnSeriesDtos());
        }

        public override ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers)
        {
            if (_getReturnSeriesSetups.ContainsKey(entityNumbers))
                return _getReturnSeriesSetups[entityNumbers];

            return new ReturnSeriesDto[0];
        }
    }
}