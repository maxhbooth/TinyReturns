using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.TinyReturnsDatabase;

namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public class CitiReturnSeriesImporter
    {
        private readonly IReturnsSeriesDataTableGateway _returnsSeriesDataTableGateway;
        private readonly ICitiReturnsFileReader _citiReturnsFileReader;
        private readonly IMonthlyReturnsDataTableGateway _monthlyReturnsDataTableGateway;

        public CitiReturnSeriesImporter(
            IReturnsSeriesDataTableGateway returnsSeriesDataTableGateway,
            ICitiReturnsFileReader citiReturnsFileReader,
            IMonthlyReturnsDataTableGateway monthlyReturnsDataTableGateway)
        {
            _monthlyReturnsDataTableGateway = monthlyReturnsDataTableGateway;
            _citiReturnsFileReader = citiReturnsFileReader;
            _returnsSeriesDataTableGateway = returnsSeriesDataTableGateway;
        }

        public virtual void DeleteAllReturns()
        {
            _monthlyReturnsDataTableGateway.DeleteAllMonthlyReturns();
            _returnsSeriesDataTableGateway.DeleteAllReturnSeries();
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
                .Select(s => s.GetPortfolioNumber())
                .Distinct();

            var returnSeries = ConvertEntityNumbersToReturnSeries(uniqueEntityNumbers, feeType);

            foreach (var series in returnSeries)
                series.ReturnSeriesId = _returnsSeriesDataTableGateway.InsertReturnSeries(series);

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
                    .Where(s => s.GetPortfolioNumber() == series.InvestmentVehicleNumber);

                var monthlyReturns = returnsForMonth
                    .Select(sourceMonthlyReturn => CreateMonthlyReturn(series, sourceMonthlyReturn))
                    .ToArray();

                _monthlyReturnsDataTableGateway.InsertMonthlyReturns(monthlyReturns);
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
            monthlyReturn.ReturnValue = sourceMonthlyReturn.GetReturnValue();

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