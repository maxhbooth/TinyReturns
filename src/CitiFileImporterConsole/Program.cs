using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.CitiFileImporterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("Console", new DatabaseSettings());

            var importer = MasterFactory.GetCitiReturnSeriesImporter();

            importer.DeleteAllReturns();

            importer.ImportMonthlyReturnsFile(args[0]);
        }
    }
}
