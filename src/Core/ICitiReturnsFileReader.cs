namespace Dimensional.TinyReturns.Core
{
    public interface ICitiReturnsFileReader
    {
        CitiReturnsRecord[] ReadFile(
            string filePath);
    }
}