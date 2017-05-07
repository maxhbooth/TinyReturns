using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.CitiFileImporterConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("Console", new DatabaseSettings());

            var interactor = MasterFactory.GetCitiReturnSeriesImporter();

            interactor.ImportMonthyPortfolioNetReturnsFile(args[0]);
            interactor.ImportMonthyPortfolioGrossReturnsFile(args[0]);
        }
    }
}
