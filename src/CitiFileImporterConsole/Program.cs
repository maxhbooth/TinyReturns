using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.CitiFileImporterConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("Console", new DatabaseSettings());

            var interactor = MasterFactory.GetCitiFileImportInteractor();

            var requestModel = new CitiFileImportRequestModel(args);

            interactor.ImportFiles(requestModel);
        }
    }
}
