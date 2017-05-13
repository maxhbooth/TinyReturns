namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio
{
    public interface IPortfolioToBenchmarkDataTableGateway
    {
        PortfolioToBenchmarkDto[] GetAll();

        void Insert(PortfolioToBenchmarkDto[] dtos);
    }
}