using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.Database.TinyReturnsDatabase
{
    public abstract class BaseTinyReturnsDataTableGateway : BaseDataTableGateway
    {
        private readonly ITinyReturnsDatabaseSettings _tinyReturnsDatabaseSettings;

        protected BaseTinyReturnsDataTableGateway(
            ITinyReturnsDatabaseSettings tinyReturnsDatabaseSettings,
            ISystemLog systemLog) : base(systemLog)
        {
            _tinyReturnsDatabaseSettings = tinyReturnsDatabaseSettings;
        }

        protected override string DefaultConnectionString
        {
            get { return _tinyReturnsDatabaseSettings.TinyReturnsDatabaseConnectionString; }
        }
    }
}