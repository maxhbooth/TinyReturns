using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core.CitiFileImport
{
    public class CitiFileImportInteractorTests
    {
        private readonly CitiReturnSeriesImporterSpy _citiReturnSeriesImporterSpy;

        public CitiFileImportInteractorTests()
        {
            _citiReturnSeriesImporterSpy = new CitiReturnSeriesImporterSpy();
        }

        [Fact]
        public void ImportFilesShouldDeleteFilesBeforeImport()
        {
            var interactor = CreateInteractor();

            var requestModel = CreateValidRequestModel();

            interactor.ImportFiles(requestModel);

            Assert.True(_citiReturnSeriesImporterSpy.DeleteAllReturnsWasCalled);
        }

        [Fact]
        public void ImportFilesShouldImportTheCorrectNumberOfFiles()
        {
            var interactor = CreateInteractor();

            var requestModel = CreateValidRequestModel();

            interactor.ImportFiles(requestModel);

            var importedFiles = _citiReturnSeriesImporterSpy.GetImportMonthlyReturnsFile();

            Assert.Equal(2, importedFiles.Length);
        }

        [Fact]
        public void ImportFilesShouldImportTheGivenListOfFiles()
        {
            var interactor = CreateInteractor();

            var requestModel = CreateValidRequestModel();

            interactor.ImportFiles(requestModel);

            var importedFiles = _citiReturnSeriesImporterSpy.GetImportMonthlyReturnsFile();

            AssertFileHasBeenImported(importedFiles, "file000");
            AssertFileHasBeenImported(importedFiles, "file001");
        }

        private CitiFileImportRequestModel CreateValidRequestModel()
        {
            var requestModel = new CitiFileImportRequestModel(new[] {"file000", "file001"});
            return requestModel;
        }

        private CitiFileImportInteractor CreateInteractor()
        {
            return new CitiFileImportInteractor(_citiReturnSeriesImporterSpy);
        }

        private void AssertFileHasBeenImported(string[] importedFiles, string file000)
        {
            Assert.NotNull(importedFiles.FirstOrDefault(f => f == file000));
        }

        private class CitiReturnSeriesImporterSpy : CitiReturnSeriesImporter
        {
            private bool _deleteAllReturnsWasCalled;
            private readonly List<string> _importMonthlyReturnsFileList;

            public CitiReturnSeriesImporterSpy()
                : base(null, null, null)
            {
                _importMonthlyReturnsFileList = new List<string>();
            }

            public bool DeleteAllReturnsWasCalled
            {
                get { return _deleteAllReturnsWasCalled; }
            }

            public override void DeleteAllReturns()
            {
                _deleteAllReturnsWasCalled = true;
            }

            public string[] GetImportMonthlyReturnsFile()
            {
                return _importMonthlyReturnsFileList.ToArray();
            }

            public override void ImportMonthlyReturnsFile(string filePath)
            {
                _importMonthlyReturnsFileList.Add(filePath);
            }
        }
    }
}