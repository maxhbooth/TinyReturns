using System.IO;
using Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles;

namespace Dimensional.TinyReturns.FileIo
{
    public class FlatFileIo : IFlatFileIo
    {
        private StreamWriter _streamWriter;

        public void OpenFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            _streamWriter = fileInfo.CreateText();
        }

        public void WriteLine(string line)
        {
            _streamWriter.WriteLine(line);
        }

        public void CloseFile()
        {
            if (_streamWriter != null)
                _streamWriter.Close();
        }
    }
}