using System.Linq;
using Headspring;

namespace Dimensional.TinyReturns.Core
{
    public class FeeType : Enumeration<FeeType>
    {
        public static FeeType None = new FeeType(1, "Not Applicable", '0');
        public static FeeType NetOfFees = new FeeType(2, "Net of Fees", 'N');
        public static FeeType GrossOfFees = new FeeType(3, "Gross of Fees", 'G');

        private readonly char _code;

        public FeeType(
            int value,
            string displayName,
            char code)
            : base(value, displayName)
        {
            _code = code;
        }

        public char Code
        {
            get { return _code; }
        }

        public static FeeType FromCode(
            char code)
        {
            var feeTypes = GetAll();

            return feeTypes.FirstOrDefault(e => e.Code == code);
        }

        public static FeeType GetFeeTypeForFileName(
            string filePath)
        {
            var feeType = None;

            if (filePath.Contains("Net"))
                feeType = NetOfFees;

            if (filePath.Contains("Gross"))
                feeType = GrossOfFees;

            return feeType;
        }
    }
}