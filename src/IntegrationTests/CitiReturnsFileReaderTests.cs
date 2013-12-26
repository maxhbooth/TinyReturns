using System.IO;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class CitiReturnsFileReaderTests : IntegrationTestBase
    {
        [Fact]
        public void ShouldReadCorrectNumberOfRecordsGivenValidReturnsFile()
        {
            var results = ReadTestFile();

            Assert.Equal(results.Length, 9);
        }

        [Fact]
        public void ShouldReadCorrectPropertiesGivenValidReturnsFile()
        {
            var results = ReadTestFile();

            Assert.Equal(results[0].ExternalId, "100");
            Assert.Equal(results[0].EndDate, "10/31/2013");
            Assert.Equal(results[0].Value, "4.40055");
        }

        private CitiReturnsRecord[] ReadTestFile()
        {
            var file = GetNetReturnsTestFilePath();
            var reader = MasterFactory.GetCitiReturnsFileReader();
            var results = reader.ReadFile(file);
            return results;
        }

        private string GetNetReturnsTestFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\NetReturns.csv";

            return targetFile;
        }

    }
}