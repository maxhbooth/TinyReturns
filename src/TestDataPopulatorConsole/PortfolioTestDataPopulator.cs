﻿using System;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class PortfolioTestDataPopulator
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

        public PortfolioTestDataPopulator(
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

            var random = new Random();

            foreach (var portfolioDto in portfolioDtos)
            {
                InsertMonthlyReturnSeries(portfolioDto, previousMonthYear, 'N');

                var next = random.Next(1, 10);

                if (next < 9) // 80% of the time
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