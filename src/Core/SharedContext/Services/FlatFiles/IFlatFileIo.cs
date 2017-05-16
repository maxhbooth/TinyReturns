namespace Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles
{
    public interface IFlatFileIo
    {
        void OpenFile(string fileName);
        void WriteLine(string line);
        void CloseFile();
    }
}