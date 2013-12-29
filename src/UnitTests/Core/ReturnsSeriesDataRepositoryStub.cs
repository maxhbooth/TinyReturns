using System;
using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class ReturnsSeriesDataRepositoryStub : IReturnsSeriesDataRepository
    {
        private readonly Dictionary<int[], ReturnSeriesDto[]> _getReturnSeriesSetups;
        private Dictionary<int[], MonthlyReturnDto[]> _getMonthlyReturnsSetups;

        public ReturnsSeriesDataRepositoryStub()
        {
            _getReturnSeriesSetups = new Dictionary<int[], ReturnSeriesDto[]>(new IntArrayEqualityComparer());
            _getMonthlyReturnsSetups = new Dictionary<int[], MonthlyReturnDto[]>(new IntArrayEqualityComparer());
        }

        public void SetupGetReturnSeries(
            int[] entityNumbers,
            Action<ReturnSeriesDtoCollectionForTests> listAction)
        {
            var col = new ReturnSeriesDtoCollectionForTests();

            listAction(col);

            _getReturnSeriesSetups.Add(entityNumbers, col.GetReturnSeriesDtos());
        }

        public ReturnSeriesDto[] GetReturnSeries(int[] entityNumbers)
        {
            if (_getReturnSeriesSetups.ContainsKey(entityNumbers))
                return _getReturnSeriesSetups[entityNumbers];

            return new ReturnSeriesDto[0];
        }

        // **

        public void SetupGetMonthlyReturns(
            int[] returnSeriesIds,
            Action<MonthlyReturnDtoCollectionForTests> listAction)
        {
            var col = new MonthlyReturnDtoCollectionForTests();

            listAction(col);

            _getMonthlyReturnsSetups.Add(returnSeriesIds, col.GetReturnSeriesDtos());
        }

        public MonthlyReturnDto[] GetMonthlyReturns(int[] returnSeriesIds)
        {
            if (_getMonthlyReturnsSetups.ContainsKey(returnSeriesIds))
                return _getMonthlyReturnsSetups[returnSeriesIds];

            return new MonthlyReturnDto[0];
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