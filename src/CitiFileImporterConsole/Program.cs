using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.CitiFileImport;
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

            ImportAllFilesFromCommandLine(args, importer);
        }

        private static void ImportAllFilesFromCommandLine(
            string[] commandLineArgs,
            CitiReturnSeriesImporter importer)
        {
            foreach (var filePath in commandLineArgs)
                importer.ImportMonthlyReturnsFile(filePath);
        }
    }
}
