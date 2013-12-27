using System.Collections.Generic;
using System.Linq;

namespace Dimensional.TinyReturns.Core
{
    public class CitiReturnSeriesImporter
    {
        private readonly IReturnsSeriesRepository _returnsSeriesRepository;
        private readonly ICitiReturnsFileReader _citiReturnsFileReader;

        public CitiReturnSeriesImporter(
            IReturnsSeriesRepository returnsSeriesRepository,
            ICitiReturnsFileReader citiReturnsFileReader)
        {
            _citiReturnsFileReader = citiReturnsFileReader;
            _returnsSeriesRepository = returnsSeriesRepository;
        }

        public void ImportPortfolioNetReturnSeriesFiles(
            string filePath)
        {
            var sourceMonthlyReturns = _citiReturnsFileReader.ReadFile(filePath);

            var entityNumbers = GetDistEntityNumbers(sourceMonthlyReturns);

            var returnSeries = entityNumbers
                .Select(CreateReturnSeries)
                .ToArray();

            returnSeries = InsertReturnSeries(returnSeries);

            InsertMonthlyReturns(returnSeries, sourceMonthlyReturns);
        }

        private void InsertMonthlyReturns(
            ReturnSeries[] returnSeries,
            CitiReturnsRecord[] sourceMonthlyReturns)
        {
            foreach (var series in returnSeries)
            {
                var returnsForMonth = sourceMonthlyReturns
                    .Where(s => s.GetConvertedExternalId() == series.EntityNumber);

                var monthlyReturns = returnsForMonth
                    .Select(sourceMonthlyReturn => CreateMonthlyReturn(series, sourceMonthlyReturn))
                    .ToArray();

                _returnsSeriesRepository.InsertMonthlyReturns(monthlyReturns);
            }
        }

        private static MonthlyReturn CreateMonthlyReturn(
            ReturnSeries series,
            CitiReturnsRecord sourceMonthlyReturn)
        {
            var monthlyReturn = new MonthlyReturn();

            monthlyReturn.ReturnSeriesId = series.ReturnSeriesId;
            monthlyReturn.Month = sourceMonthlyReturn.GetMonth();
            monthlyReturn.Year = sourceMonthlyReturn.GetYear();
            monthlyReturn.ReturnValue = sourceMonthlyReturn.GetDecimalValue();

            return monthlyReturn;
        }

        private ReturnSeries[] InsertReturnSeries(
            ReturnSeries[] returnSeries)
        {
            foreach (var series in returnSeries)
                series.ReturnSeriesId = _returnsSeriesRepository.InsertReturnSeries(series);

            return returnSeries;
        }

        private static ReturnSeries CreateReturnSeries
            (int entityNumber)
        {
            return new ReturnSeries()
            {
                EntityNumber = entityNumber,
                FeeTypeCode = 'N'
            };
        }

        private IEnumerable<int> GetDistEntityNumbers(
            IEnumerable<CitiReturnsRecord> sourceMonthlyReturns)
        {
            var entityNumbers = sourceMonthlyReturns
                .Select(s => s.GetConvertedExternalId())
                .Distinct();
            return entityNumbers;
        }
    }
}