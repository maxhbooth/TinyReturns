using System;
using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Core.ReturnSeriesImportContext.Services.CitiFileImport
{
    public class CitiMonthyReturnImporter
    {
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
        private readonly ICitiReturnsFileReader _citiReturnsFileReader;
        private readonly IMonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;

        public CitiMonthyReturnImporter(
            ICitiReturnsFileReader citiReturnsFileReader,
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway,
            IMonthlyReturnDataTableGateway monthlyReturnDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway)
        {
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _monthlyReturnDataTableGateway = monthlyReturnDataTableGateway;
            _citiReturnsFileReader = citiReturnsFileReader;
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
        }

        public void ImportMonthyPortfolioNetReturnsFile(string filePath)
        {
            ImportFile(filePath, PortfolioToReturnSeriesDto.NetSeriesTypeCode);
        }

        public void ImportMonthyPortfolioGrossReturnsFile(string filePath)
        {
            ImportFile(filePath, PortfolioToReturnSeriesDto.GrossSeriesTypeCode);
        }

        private void ImportFile(
            string filePath,
            char netSeriesTypeCode)
        {
            var citiMonthlyReturnsDataFileRecords = _citiReturnsFileReader.ReadFile(filePath);

            var returnSeriesDtos = new List<ReturnSeriesDto>();

            var monthlyReturnDtos = new List<MonthlyReturnDto>();

            foreach (var citiMonthlyReturnsDataFileRecord in citiMonthlyReturnsDataFileRecords)
            {
                var portfolioNumber = citiMonthlyReturnsDataFileRecord.GetPortfolioNumber();

                var returnSeriesName = CreateReturnSeriesName(portfolioNumber);

                var returnSeriesDto = returnSeriesDtos.FirstOrDefault(d => d.Name == returnSeriesName);

                if (returnSeriesDto == null)
                {
                    returnSeriesDto = CreateAndInsertReturnSeriesDto(returnSeriesName);

                    returnSeriesDtos.Add(returnSeriesDto);

                    InsertPortfolioToReturnSeriesRecord(
                        portfolioNumber,
                        returnSeriesDto.ReturnSeriesId, netSeriesTypeCode);
                }

                monthlyReturnDtos.Add(new MonthlyReturnDto()
                {
                    ReturnSeriesId = returnSeriesDto.ReturnSeriesId,
                    Year = citiMonthlyReturnsDataFileRecord.GetYear(),
                    Month = citiMonthlyReturnsDataFileRecord.GetMonth(),
                    ReturnValue = citiMonthlyReturnsDataFileRecord.GetReturnValue()
                });
            }

            _monthlyReturnDataTableGateway.Insert(monthlyReturnDtos.ToArray());
        }

        private void InsertPortfolioToReturnSeriesRecord(
            int portfolioNumber,
            int seriesId,
            char netSeriesTypeCode)
        {
            var portfolioToReturnSeriesDto = new PortfolioToReturnSeriesDto()
            {
                PortfolioNumber = portfolioNumber,
                ReturnSeriesId = seriesId,
                SeriesTypeCode = netSeriesTypeCode
            };

            _portfolioToReturnSeriesDataTableGateway.Insert(new[]
            {
                portfolioToReturnSeriesDto,
            });
        }

        private ReturnSeriesDto CreateAndInsertReturnSeriesDto(
            string returnSeriesName)
        {
            var returnSeriesDto = new ReturnSeriesDto()
            {
                Name = returnSeriesName,
                Disclosure = String.Empty
            };

            var returnSeriesId = _returnSeriesDataTableGateway.Insert(returnSeriesDto);

            returnSeriesDto.ReturnSeriesId = returnSeriesId;

            return returnSeriesDto;
        }

        public static string CreateReturnSeriesName(int portfolioNumber)
        {
            return string.Format("Return Series for Portfolio Number {0}", portfolioNumber);
        }
    }
}