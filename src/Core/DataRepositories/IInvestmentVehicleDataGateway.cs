namespace Dimensional.TinyReturns.Core.DataRepositories
{
    public interface IInvestmentVehicleDataRepository
    {
        InvestmentVehicleDto[] GetAllEntities();
    }
}