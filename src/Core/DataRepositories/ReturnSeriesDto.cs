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
                EntityNumber = entityNumber,
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
                EntityNumber = entityNumber,
                FeeTypeCode = FeeType.GrossOfFees.Code
            };
        }

        public int ReturnSeriesId { get; set; }
        public int EntityNumber { get; set; }
        public char FeeTypeCode { get; set; }
    }
}