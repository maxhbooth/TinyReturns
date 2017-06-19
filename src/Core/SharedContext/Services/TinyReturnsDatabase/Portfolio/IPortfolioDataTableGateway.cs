namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio
{
    public interface IPortfolioDataTableGateway
    {
        void Insert(PortfolioDto dto);
        void Update(PortfolioDto dto);

        PortfolioDto[] GetAll();
    }
}