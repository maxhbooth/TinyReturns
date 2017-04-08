namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase
{
    public interface IInvestmentVehicleDataGateway
    {
        InvestmentVehicleDto[] GetAllEntities();
    }
}