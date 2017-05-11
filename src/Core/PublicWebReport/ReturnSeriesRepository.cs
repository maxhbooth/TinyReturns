using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class ReturnSeriesRepository
    {
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;

        public ReturnSeriesRepository(
            IReturnSeriesDataTableGateway returnSeriesDataGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway)
        {
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _returnSeriesDataGateway = returnSeriesDataGateway;
        }

        public ReturnSeries[] GetReturnSeries(
            int[] targetReturnSeriesIds)
        {
            var returnSeriesDtos = _returnSeriesDataGateway.Get(targetReturnSeriesIds);

            var monthlyReturnDtos = _monthlyReturnDataTableGateway.Get(targetReturnSeriesIds);

            var returnSeries = new List<ReturnSeries>();

            foreach (var returnSeriesDto in returnSeriesDtos)
            {
                var returnDtos = monthlyReturnDtos.Where(d => d.ReturnSeriesId == returnSeriesDto.ReturnSeriesId);

                var monthlyReturns = new List<ReturnSeries.MonthlyReturn>();

                foreach (var monthlyReturnDto in returnDtos)
                {
                    monthlyReturns.Add(new ReturnSeries.MonthlyReturn(
                        new MonthYear(monthlyReturnDto.Year, monthlyReturnDto.Month), monthlyReturnDto.ReturnValue));
                }

                var series = new ReturnSeries(returnSeriesDto.ReturnSeriesId, monthlyReturns.ToArray());


                returnSeries.Add(series);
            }

            return returnSeries.ToArray();
        }
    }
}