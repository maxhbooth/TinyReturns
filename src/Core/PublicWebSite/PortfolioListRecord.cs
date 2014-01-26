namespace Dimensional.TinyReturns.Core.PublicWebSite
{
    public class PortfolioListRecord
    {
        public int PortfolioNumber { get; set; }
        public string PortfolioName { get; set; }

        public SerializableReturnResult OneMonth { get; set; }
        public SerializableReturnResult ThreeMonth { get; set; }
        public SerializableReturnResult YearToDate { get; set; }
    }
}