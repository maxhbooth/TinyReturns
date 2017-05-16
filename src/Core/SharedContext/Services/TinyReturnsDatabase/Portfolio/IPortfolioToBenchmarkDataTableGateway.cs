namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio
{
    public interface IPortfolioToBenchmarkDataTableGateway
    {
        PortfolioToBenchmarkDto[] GetAll();

        void Insert(PortfolioToBenchmarkDto[] dtos);
    }
}