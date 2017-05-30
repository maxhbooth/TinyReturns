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

#if DEBUG
            interactor.ImportMonthyPortfolioNetReturnsFile("CitiFileFullGrossOfFees.csv");
            interactor.ImportMonthyPortfolioGrossReturnsFile("CitiFileFullNetOfFees.csv");
#else
            interactor.ImportMonthyPortfolioNetReturnsFile(args[0]);
            interactor.ImportMonthyPortfolioGrossReturnsFile(args[1]);
#endif
        }
    }
}
