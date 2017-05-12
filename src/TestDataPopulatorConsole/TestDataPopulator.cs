using System;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class TestDataPopulator
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

        public TestDataPopulator(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway)
        {
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }

        public void PopulateTestData()
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            var monthYear = new MonthYear(DateTime.Now);
            var previousMonthYear = monthYear.AddMonths(-1);

            foreach (var portfolioDto in portfolioDtos)
            {
                InsertMonthlyReturnSeries(portfolioDto, previousMonthYear, 'N');
                InsertMonthlyReturnSeries(portfolioDto, previousMonthYear, 'G', 1);
            }
        }

        private void InsertMonthlyReturnSeries(
            PortfolioDto portfolioDto,
            MonthYear previousMonthYear,
            char feeTypeCode,
            int seedOffSet = 0)
        {
            var feeTypeName = "Net of Fees";

            if (feeTypeCode == 'G')
                feeTypeName = "Gross of Fees";

            var netReturnSeriesId = _returnSeriesDataTableGateway.Insert(new ReturnSeriesDto()
            {
                Name = string.Format("{0} - {1}", portfolioDto.Name, feeTypeName),
                Disclosure = string.Empty
            });

            var monthYearRange = new MonthYearRange(
                new MonthYear(portfolioDto.InceptionDate),
                previousMonthYear);

            if (portfolioDto.CloseDate.HasValue)
            {
                monthYearRange = new MonthYearRange(
                    new MonthYear(portfolioDto.InceptionDate),
                    new MonthYear(portfolioDto.CloseDate.Value));
            }

            var monthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                netReturnSeriesId,
                monthYearRange,
                seed: portfolioDto.Number + seedOffSet);

            _monthlyReturnDataTableGateway.Insert(monthlyReturnDtos);

            var portfolioToReturnSeriesDto = new PortfolioToReturnSeriesDto()
            {
                PortfolioNumber = portfolioDto.Number,
                ReturnSeriesId = netReturnSeriesId,
                SeriesTypeCode = feeTypeCode
            };

            _portfolioToReturnSeriesDataTableGateway.Insert(new []{ portfolioToReturnSeriesDto,  });
        }
    }
}