namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio
{
    public interface IPortfolioDataTableGateway
    {
        void Insert(PortfolioDto dto);

        PortfolioDto[] GetAll();
    }
}