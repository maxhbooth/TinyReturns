namespace Dimensional.TinyReturns.Core.PublicWebReport
{
    //TODO: Not sure if we need this class.
    public class MonthlyReturnFindResult
    {
        public static MonthlyReturnFindResult NotFoundResult = new MonthlyReturnFindResult(false, 0);

        public MonthlyReturnFindResult(
            bool wasFound,
            decimal value)
        {
            Value = value;
            WasFound = wasFound;
        }

        public bool WasFound { get; }
        public decimal Value { get; }
    }
}