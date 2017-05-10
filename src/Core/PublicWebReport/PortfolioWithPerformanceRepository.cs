using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    public class PortfolioWithPerformanceRepository
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;

        public PortfolioWithPerformanceRepository(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway,
            IReturnSeriesDataTableGateway returnSeriesDataGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway)
        {
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _returnSeriesDataGateway = returnSeriesDataGateway;
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }

        public PortfolioWithPerformance[] GetAll()
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            var portfolioModels = new List<PortfolioWithPerformance>();

            var portfolioNumbers = portfolioDtos.Select(p => p.Number).ToArray();

            var portfolioToReturnSeriesDtos = _portfolioToReturnSeriesDataTableGateway.Get(portfolioNumbers);

            var targetReturnSeriesIds = portfolioToReturnSeriesDtos
                .Select(d => d.ReturnSeriesId)
                .ToArray();

            var returnSeries = GetReturnSeries(targetReturnSeriesIds);
            
            foreach (var portfolioDto in portfolioDtos)
            {
                var netDto = portfolioToReturnSeriesDtos.FirstOrDefault(
                    d =>
                        d.PortfolioNumber == portfolioDto.Number &&
                        d.SeriesTypeCode == PortfolioToReturnSeriesDto.NetSeriesTypeCode);

                var grossDto = portfolioToReturnSeriesDtos.FirstOrDefault(
                    d =>
                        d.PortfolioNumber == portfolioDto.Number &&
                        d.SeriesTypeCode == PortfolioToReturnSeriesDto.GrossSeriesTypeCode);

                ReturnSeries netReturnSeries = null;
                ReturnSeries grossReturnSeries = null;

                if (netDto != null)
                    netReturnSeries = returnSeries.FirstOrDefault(r => r.Id == netDto.ReturnSeriesId);

                if (grossDto != null)
                    grossReturnSeries = returnSeries.FirstOrDefault(r => r.Id == grossDto.ReturnSeriesId);

                portfolioModels.Add(new PortfolioWithPerformance(portfolioDto.Number, portfolioDto.Name, netReturnSeries, grossReturnSeries));
            }

            return portfolioModels.ToArray();
        }

        private ReturnSeries[] GetReturnSeries(
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