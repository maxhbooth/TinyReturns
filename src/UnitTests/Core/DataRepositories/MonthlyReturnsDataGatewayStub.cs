using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core.DataRepositories
{
    public class MonthlyReturnsDataGatewayStub : IMonthlyReturnsDataGateway
    {
        private readonly List<MonthlyReturnDto> _monthlyReturnDtos;

        public MonthlyReturnsDataGatewayStub()
        {
            _monthlyReturnDtos = new List<MonthlyReturnDto>();
        }

        public void InsertMonthlyReturns(MonthlyReturnDto[] monthlyReturns)
        {
            _monthlyReturnDtos.AddRange(monthlyReturns);
        }

        public MonthlyReturnDto[] GetMonthlyReturns(int returnSeriesId)
        {
            return _monthlyReturnDtos
                .Where(dto => dto.ReturnSeriesId == returnSeriesId)
                .ToArray();
        }

        public MonthlyReturnDto[] GetMonthlyReturns(int[] returnSeriesIds)
        {
            return _monthlyReturnDtos
                .Where(dto => returnSeriesIds.Any(id => dto.ReturnSeriesId == id))
                .ToArray();
        }

        public void DeleteMonthlyReturns(int returnSeriesId)
        {
            _monthlyReturnDtos.RemoveAll(dto => dto.ReturnSeriesId == returnSeriesId);
        }

        public void DeleteAllMonthlyReturns()
        {
            _monthlyReturnDtos.Clear();
        }
    }
}