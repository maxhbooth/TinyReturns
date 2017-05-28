using System;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class BenchmarkTestDataPopulator
    {
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly IBenchmarkDataTableGateway _benchmarkDataTableGateway;
        private readonly IBenchmarkToReturnSeriesDataTableGateway _benchmarkToReturnSeriesDataTableGateway;

        public BenchmarkTestDataPopulator(
            IBenchmarkDataTableGateway benchmarkDataTableGateway,
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway,
            IBenchmarkToReturnSeriesDataTableGateway benchmarkToReturnSeriesDataTableGateway)
        {
            _benchmarkToReturnSeriesDataTableGateway = benchmarkToReturnSeriesDataTableGateway;
            _benchmarkDataTableGateway = benchmarkDataTableGateway;
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
        }

        public void PopulateTestData()
        {
            var benchmarkDtos = _benchmarkDataTableGateway.GetAll();

            var monthYear = new MonthYear(DateTime.Now);
            var previousMonthYear = monthYear.AddMonths(-1);

            var random = new Random();

            var numberOfMonthsBack = random.Next(1, 240) + 120;

            foreach (var benchmarkDto in benchmarkDtos)
            {
                var monthYearRange = new MonthYearRange(
                    previousMonthYear.AddMonths(numberOfMonthsBack * -1),
                    previousMonthYear);

                InsertMonthlyReturnSeries(
                    benchmarkDto,
                    monthYearRange);
            }
        }

        private void InsertMonthlyReturnSeries(
            BenchmarkDto benchmarkDto,
            MonthYearRange monthYearRange,
            int seedOffSet = 0)
        {
            var netReturnSeriesId = _returnSeriesDataTableGateway.Insert(new ReturnSeriesDto()
            {
                Name = string.Format("{0}", benchmarkDto.Name)
            });

            var monthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                netReturnSeriesId,
                monthYearRange,
                seed: benchmarkDto.Number + seedOffSet);

            _monthlyReturnDataTableGateway.Insert(monthlyReturnDtos);

            var benchmarkToReturnSeriesDto = new BenchmarkToReturnSeriesDto()
            {
                BenchmarkNumber = benchmarkDto.Number,
                ReturnSeriesId = netReturnSeriesId
            };

            _benchmarkToReturnSeriesDataTableGateway.Insert(new[] { benchmarkToReturnSeriesDto, });
        }


    }
}