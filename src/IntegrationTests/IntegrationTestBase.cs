using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class IntegrationTestBase
    {
        private static bool _isBootstrapped = false;
        
        public IntegrationTestBase()
        {
            if (!_isBootstrapped)
            {
                DependencyManager.BootstrapForTests(
                    new SystemLogForIntegrationTests(),
                    new DatabaseSettings());

                _isBootstrapped = true;
            }
        }
    }
}