using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class FlatFileIoSpy : IFlatFileIo
    {
        private string _openFileName;
        private bool _closedFileWasCalled;

        private readonly List<string> _lines;

        public FlatFileIoSpy()
        {
            _closedFileWasCalled = false;
            _lines = new List<string>();
        }

        public string FirstLine()
        {
            return _lines.FirstOrDefault();
        }

        public bool ContainsLine(
            string line)
        {
            return _lines.Any(l => l == line);
        }

        public int NumberOfLines
        {
            get { return _lines.Count; }
        }

        public bool WasLineAdded(
            string line)
        {
            return _lines.Any(l => l == line);
        }

        public string OpenFileName
        {
            get { return _openFileName; }
        }

        public bool ClosedFileWasCalled
        {
            get { return _closedFileWasCalled; }
        }

        public void OpenFile(string fileName)
        {
            _openFileName = fileName;
        }

        public void WriteLine(string line)
        {
            _lines.Add(line);
        }

        public void CloseFile()
        {
            _closedFileWasCalled = true;
        }
    }
}