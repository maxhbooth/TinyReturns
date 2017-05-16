namespace Dimensional.TinyReturns.Core.ReturnSeriesImportContext.Services.CitiFileImport
{
    public interface ICitiReturnsFileReader
    {
        CitiMonthlyReturnsDataFileRecord[] ReadFile(
            string filePath);
    }
}