namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public class ReturnSeriesDto
    {
        public static ReturnSeriesDto CreateForNetOfFees(
            int returnSeriesId,
            int entityNumber)
        {
            return new ReturnSeriesDto()
            {
                ReturnSeriesId = returnSeriesId,
                InvestmentVehicleNumber = entityNumber,
                FeeTypeCode = FeeType.NetOfFees.Code
            };
        }

        public static ReturnSeriesDto CreateForGrossOfFees(
            int returnSeriesId,
            int entityNumber)
        {
            return new ReturnSeriesDto()
            {
                ReturnSeriesId = returnSeriesId,
                InvestmentVehicleNumber = entityNumber,
                FeeTypeCode = FeeType.GrossOfFees.Code
            };
        }

        public int ReturnSeriesId { get; set; }
        public int InvestmentVehicleNumber { get; set; }
        public char FeeTypeCode { get; set; }
    }
}