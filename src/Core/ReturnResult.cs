namespace Dimensional.TinyReturns.Core
{
    public class ReturnResult
    {
        private bool _hasError;
        private decimal _value;

        public bool HasError
        {
            get { return _hasError; }
        }

        public decimal Value
        {
            get { return _value; }
        }

        public void SetError()
        {
            _hasError = true;
        }

        public void SetValue(decimal value)
        {
            _hasError = false;
            _value = value;
        }
    }
}