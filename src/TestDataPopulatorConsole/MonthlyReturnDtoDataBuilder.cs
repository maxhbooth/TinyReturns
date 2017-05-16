using System;
using System.Collections.Generic;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.TestDataPopulatorConsole
{
    public class MonthlyReturnDtoDataBuilder
    {
        public static MonthlyReturnDto[] CreateMonthlyReturns(
            int returnSeriesId,
            MonthYearRange monthYearRange,
            int seed = 0)
        {
            var random = new Random(seed);

            var monthlyReturnDtos = new List<MonthlyReturnDto>();

            monthYearRange.ForEachMonthInRange((monthYear) =>
            {
                var nextRandomValue = random.Next(-10000, 10000) / 10000m;

                monthlyReturnDtos.Add(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesId,
                    Year = monthYear.Year,
                    Month = monthYear.Month,
                    ReturnValue = nextRandomValue
                });
            });

            return monthlyReturnDtos.ToArray();
        }
    }
}