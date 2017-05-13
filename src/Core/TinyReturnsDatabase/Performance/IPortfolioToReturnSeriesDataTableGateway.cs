namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IPortfolioToReturnSeriesDataTableGateway
    {
        PortfolioToReturnSeriesDto[] GetAll();

        PortfolioToReturnSeriesDto[] Get(int[] portfolioNumbers);

        void Insert(PortfolioToReturnSeriesDto[] dtos);
    }
}