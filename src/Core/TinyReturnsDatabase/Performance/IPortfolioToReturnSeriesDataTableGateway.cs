namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Performance
{
    public interface IPortfolioToReturnSeriesDataTableGateway
    {
        PortfolioToReturnSeriesDto[] GetAll();

        void Insert(PortfolioToReturnSeriesDto[] dtos);
    }
}