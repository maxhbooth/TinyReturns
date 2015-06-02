using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public class CitiReturnSeriesImporter
    {
        private readonly IReturnsSeriesDataGateway _returnsSeriesDataGateway;
        private readonly ICitiReturnsFileReader _citiReturnsFileReader;
        private readonly IMonthlyReturnsDataGateway _monthlyReturnsDataGateway;

        public CitiReturnSeriesImporter(
            IReturnsSeriesDataGateway returnsSeriesDataGateway,
            ICitiReturnsFileReader citiReturnsFileReader,
            IMonthlyReturnsDataGateway monthlyReturnsDataGateway)
        {
            _monthlyReturnsDataGateway = monthlyReturnsDataGateway;
            _citiReturnsFileReader = citiReturnsFileReader;
            _returnsSeriesDataGateway = returnsSeriesDataGateway;
        }

        public virtual void DeleteAllReturns()
        {
            _monthlyReturnsDataGateway.DeleteAllMonthlyReturns();
            _returnsSeriesDataGateway.DeleteAllReturnSeries();
        }

        public virtual void ImportMonthlyReturnsFile(
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
                series.ReturnSeriesId = _returnsSeriesDataGateway.InsertReturnSeries(series);

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
                    .Where(s => s.GetConvertedExternalId() == series.InvestmentVehicleNumber);

                var monthlyReturns = returnsForMonth
                    .Select(sourceMonthlyReturn => CreateMonthlyReturn(series, sourceMonthlyReturn))
                    .ToArray();

                _monthlyReturnsDataGateway.InsertMonthlyReturns(monthlyReturns);
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
                InvestmentVehicleNumber = entityNumber,
                FeeTypeCode = feeTypeCode
            };
        }
    }
}