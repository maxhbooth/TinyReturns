using Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public class CitiMonthyReturnImporter
    {
        private readonly IReturnSeriesDataTableGateway _returnSeriesDataTableGateway;

        public CitiMonthyReturnImporter(
            IReturnSeriesDataTableGateway returnSeriesDataTableGateway)
        {
            _returnSeriesDataTableGateway = returnSeriesDataTableGateway;
        }

        public void ImportMonthyPortfolioNetReturnsFile(
            string filePath)
        {
            _returnSeriesDataTableGateway.Inert(new ReturnSeriesDto()
            {
                Name = "Returns for Portfolio 100",
                Disclosure = string.Empty
            });
        }
    }
}