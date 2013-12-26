using System.IO;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class ReturnSeriesImportTests
    {
        [Fact]
        public void ShouldImportDataWhenGivenValidFile()
        {
            var importer = new ReturnSeriesImporter();

//            importer.ImportReturnSeries();
        }

        private string GetTestDataPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\IntegrationTests\NetReturns.csv";

            return targetFile;
        }

    }

    public class ReturnSeriesImporter
    {
    }
}