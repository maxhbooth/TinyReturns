namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class ReturnResult
    {
        public static ReturnResult CreateWithError(
            string message)
        {
            var r = new ReturnResult();
            r.SetError(message);
            return r;
        }

        public static ReturnResult CreateWithValue(
            decimal value,
            string calculation)
        {
            var r = new ReturnResult();
            r.SetValue(value, calculation);
            return r;
        }

        private bool _hasError;
        private decimal _value;
        private string _errorMessage;
        private string _calculation;

        public bool HasError
        {
            get { return _hasError; }
        }

        public decimal Value
        {
            get { return _value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public string Calculation
        {
            get { return _calculation; }
        }

        public void SetError(
            string message)
        {
            _hasError = true;
            _errorMessage = message;
        }

        public void SetValue(
            decimal value,
            string calculation)
        {
            _hasError = false;
            _value = value;
            _calculation = calculation;
        }

        public void AppendToCalculation(
            decimal newValue,
            string moreCalculation)
        {
            _hasError = false;
            _value = newValue;
            _calculation += System.Environment.NewLine + moreCalculation;
        }

        public decimal? GetNullValueOnError()
        {
            if (_hasError)
                return null;

            return _value;
        }
    }
}