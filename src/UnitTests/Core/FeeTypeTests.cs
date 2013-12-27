using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class FeeTypeTests
    {
        [Fact]
        public void GetFeeTypeForFileNameShouldReturnNoneByDefault()
        {
            Assert.Equal(FeeType.GetFeeTypeForFileName(""), FeeType.None);
        }

        [Fact]
        public void GetFeeTypeForFileNameShouldReturnNetOfFeeWhenFileNameContainsNet()
        {
            Assert.Equal(FeeType.GetFeeTypeForFileName("FileNameNet"), FeeType.NetOfFees);
        }

        [Fact]
        public void GetFeeTypeForFileNameShouldReturnGrossOfFeeWhenFileNameContainsGross()
        {
            Assert.Equal(FeeType.GetFeeTypeForFileName("FileNameGross"), FeeType.GrossOfFees);
        }
    }
}