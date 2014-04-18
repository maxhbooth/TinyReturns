namespace Dimensional.TinyReturns.Core.CitiFileImport
{
    public interface ICitiFileImportInteractor
    {
        void ImportFiles(CitiFileImportRequestModel requestModel);
    }

    public class CitiFileImportInteractor : ICitiFileImportInteractor
    {
        private readonly CitiReturnSeriesImporter _citiReturnSeriesImporter;

        public CitiFileImportInteractor(
            CitiReturnSeriesImporter citiReturnSeriesImporter)
        {
            _citiReturnSeriesImporter = citiReturnSeriesImporter;
        }

        public void ImportFiles(CitiFileImportRequestModel requestModel)
        {
            _citiReturnSeriesImporter.DeleteAllReturns();

            var files = requestModel.GetFiles();

            foreach (var file in files)
                _citiReturnSeriesImporter.ImportMonthlyReturnsFile(file);
        }
    }

    public class CitiFileImportRequestModel
    {
        private readonly string[] _files;

        public CitiFileImportRequestModel(
            string[] files)
        {
            _files = files;
        }

        public string[] GetFiles()
        {
            return _files;
        }
    }   
}