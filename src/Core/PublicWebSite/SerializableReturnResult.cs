namespace Dimensional.TinyReturns.Core.PublicWebSite
{
    public class SerializableReturnResult
    {
        public SerializableReturnResult()
        {
        }

        public SerializableReturnResult(
            ReturnResult returnResult)
        {
            if (returnResult.HasError)
            {
                HasError = returnResult.HasError;
                ErrorMessage = returnResult.ErrorMessage;
            }
            else
            {
                Value = returnResult.Value;
                Calculation = returnResult.Calculation;
            }
        }

        public decimal? Value { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string Calculation { get; set; }
    }
}