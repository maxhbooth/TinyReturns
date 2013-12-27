using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DataRepository;

namespace Dimensional.TinyReturns.Core.CitiFileImport
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

        public void ImportMonthlyReturnsFile(
            string filePath,
            FeeType feeType)
        {
            var sourceMonthlyReturns = _citiReturnsFileReader.ReadFile(filePath);

            var entityNumbers = GetDistEntityNumbers(sourceMonthlyReturns);

            var returnSeries = entityNumbers
                .Select(n => CreateReturnSeries(n, feeType.Code))
                .ToArray();

            returnSeries = InsertReturnSeries(returnSeries);

            InsertMonthlyReturns(returnSeries, sourceMonthlyReturns);
        }

        private void InsertMonthlyReturns(
            ReturnSeriesDto[] returnSeries,
            CitiMonthlyReturnsDataFileRecord[] sourceMonthlyReturns)
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

        private MonthlyReturnDto CreateMonthlyReturn(
            ReturnSeriesDto series,
            CitiMonthlyReturnsDataFileRecord sourceMonthlyReturn)
        {
            var monthlyReturn = new MonthlyReturnDto();

            monthlyReturn.ReturnSeriesId = series.ReturnSeriesId;
            monthlyReturn.Month = sourceMonthlyReturn.GetMonth();
            monthlyReturn.Year = sourceMonthlyReturn.GetYear();
            monthlyReturn.ReturnValue = sourceMonthlyReturn.GetDecimalValue();

            return monthlyReturn;
        }

        private ReturnSeriesDto[] InsertReturnSeries(
            ReturnSeriesDto[] returnSeries)
        {
            foreach (var series in returnSeries)
                series.ReturnSeriesId = _returnsSeriesRepository.InsertReturnSeries(series);

            return returnSeries;
        }

        private ReturnSeriesDto CreateReturnSeries(
            int entityNumber,
            char feeTypeCode)
        {
            return new ReturnSeriesDto()
            {
                EntityNumber = entityNumber,
                FeeTypeCode = feeTypeCode
            };
        }

        private IEnumerable<int> GetDistEntityNumbers(
            IEnumerable<CitiMonthlyReturnsDataFileRecord> sourceMonthlyReturns)
        {
            var entityNumbers = sourceMonthlyReturns
                .Select(s => s.GetConvertedExternalId())
                .Distinct();
            return entityNumbers;
        }
    }
}