namespace Dimensional.TinyReturns.Core.SharedContext.Services.CitiFileImport
{
    public interface ICitiReturnsFileReader
    {
        CitiMonthlyReturnsDataFileRecord[] ReadFile(
            string filePath);
    }
}