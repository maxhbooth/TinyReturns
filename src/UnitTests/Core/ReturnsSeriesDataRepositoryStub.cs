using System;
using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class ReturnsSeriesDataRepositoryStub : IReturnsSeriesDataRepository
    {
        private readonly Dictionary<int[], ReturnSeriesDto[]> _getReturnSeriesSetups;

        public ReturnsSeriesDataRepositoryStub()
        {
            _getReturnSeriesSetups = new Dictionary<int[], ReturnSeriesDto[]>(new IntArrayCompare());
        }

        public void SetupGetReturnSeries(
            int[] entityNumbers,
            Action<ReturnSeriesDtoCollection> listAction)
        {
            var col = new ReturnSeriesDtoCollection();

            listAction(col);

            _getReturnSeriesSetups.Add(entityNumbers, col.GetReturnSeriesDtos());
        }

        private class IntArrayCompare : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                var sequenceEqual = x.SequenceEqual(y);
                return sequenceEqual;
            }

            public int GetHashCode(int[] obj)
            {
                return 0;
            }
        }

        public class ReturnSeriesDtoCollection
        {
            private readonly List<ReturnSeriesDto> _returnSeriesDtoList;

            public ReturnSeriesDtoCollection()
            {
                _returnSeriesDtoList = new List<ReturnSeriesDto>();
            }

            public ReturnSeriesDtoCollection AddNetOfFeesReturnSeries(
                int returnSeriesId,
                int entityNumber)
            {
                var n = ReturnSeriesDto.CreateForNetOfFees(returnSeriesId, entityNumber);
                _returnSeriesDtoList.Add(n);

                return this;
            }

            public ReturnSeriesDtoCollection AddNetOfGrossReturnSeries(
                int returnSeriesId,
                int entityNumber)
            {
                var n = ReturnSeriesDto.CreateForGrossOfFees(returnSeriesId, entityNumber);
                _returnSeriesDtoList.Add(n);

                return this;
            }

            public ReturnSeriesDto[] GetReturnSeriesDtos()
            {
                return _returnSeriesDtoList.ToArray();
            }
        }

        public ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers)
        {
            if (_getReturnSeriesSetups.ContainsKey(entityNumbers))
                return _getReturnSeriesSetups[entityNumbers];

            return new ReturnSeriesDto[0];
        }

        // **

        public int InsertReturnSeries(ReturnSeriesDto returnSeries)
        {
            throw new System.NotImplementedException();
        }

        public ReturnSeriesDto GetReturnSeries(int returnSeriesId)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteReturnSeries(int returnSeriesId)
        {
            throw new System.NotImplementedException();
        }


        public void DeleteAllReturnSeries()
        {
            throw new System.NotImplementedException();
        }

        public void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns)
        {
            throw new System.NotImplementedException();
        }

        public MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteMonthlyReturns(int returnSeriesId)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAllMonthlyReturns()
        {
            throw new System.NotImplementedException();
        }
    }
}