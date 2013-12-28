using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public class CitiReturnSeriesImporter
    {
        private readonly IReturnsSeriesDataRepository _returnsSeriesDataRepository;
        private readonly ICitiReturnsFileReader _citiReturnsFileReader;

        public CitiReturnSeriesImporter(
            IReturnsSeriesDataRepository returnsSeriesDataRepository,
            ICitiReturnsFileReader citiReturnsFileReader)
        {
            _citiReturnsFileReader = citiReturnsFileReader;
            _returnsSeriesDataRepository = returnsSeriesDataRepository;
        }

        public void DeleteAllReturns()
        {
            _returnsSeriesDataRepository.DeleteAllMonthlyReturns();
            _returnsSeriesDataRepository.DeleteAllReturnSeries();
        }
        
        public void ImportMonthlyReturnsFile(
            string filePath)
        {
            var feeType = FeeType.GetFeeTypeForFileName(filePath);

            var citiMonthlyReturns = _citiReturnsFileReader.ReadFile(filePath);

            var returnSeries = SaveReturnSeries(feeType, citiMonthlyReturns);

            SaveMonthlyReturns(returnSeries, citiMonthlyReturns);
        }

        private ReturnSeriesDto[] SaveReturnSeries(
            FeeType feeType,
            CitiMonthlyReturnsDataFileRecord[] citiMonthlyReturns)
        {
            var uniqueEntityNumbers = citiMonthlyReturns
                .Select(s => s.GetConvertedExternalId())
                .Distinct();

            var returnSeries = ConvertEntityNumbersToReturnSeries(uniqueEntityNumbers, feeType);

            foreach (var series in returnSeries)
                series.ReturnSeriesId = _returnsSeriesDataRepository.InsertReturnSeries(series);

            return returnSeries;
        }

        private ReturnSeriesDto[] ConvertEntityNumbersToReturnSeries(
            IEnumerable<int> entityNumbers,
            FeeType feeType)
        {
            var returnSeries = entityNumbers
                .Select(n => CreateReturnSeries(n, feeType.Code))
                .ToArray();

            return returnSeries;
        }

        private void SaveMonthlyReturns(
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

                _returnsSeriesDataRepository.InsertMonthlyReturns(monthlyReturns);
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
    }
}