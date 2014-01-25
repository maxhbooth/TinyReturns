using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.OmniFileExporterConsole
{
    class Program
    {
        public static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("OmniFileExporterConsole", new DatabaseSettings());

            var omniDataFileCreator = MasterFactory.GetOmniDataFileCreator();

            omniDataFileCreator.CreateFile(new MonthYear(2012, 6));
        }
    }
}
