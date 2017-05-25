using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class FinancialMathTests
    {
        [Fact]
        public void PerformGeometricLinkingShouldReturnSingleValueAsResult()
        {
            var financialMath = new FinancialMath();

            var result = financialMath.PerformGeometricLinking(new[] {0.1m});

            Assert.Equal(0.1m, result.Value);
            Assert.Equal("0.1 = 0.1", result.Calculation);
        }

        [Fact]
        public void PerformGeometricLinkingShouldLinkingOnTwoValues()
        {
            var financialMath = new FinancialMath();

            var result = financialMath.PerformGeometricLinking(new[] { 0.1m, 0.2m });

            Assert.Equal(0.32m, result.Value);
            Assert.Equal("((1 + 0.1) * (1 + 0.2) - 1) = 0.32", result.Calculation);
        }

        [Fact]
        public void PerformGeometricLinkingShouldLinkingOnMoreThanTwoValues()
        {
            var financialMath = new FinancialMath();

            var result = financialMath.PerformGeometricLinking(new[] { 0.1m, 0.2m, 0.3m });

            Assert.Equal(0.716m, result.Value);
            Assert.Equal("((1 + ((1 + 0.1) * (1 + 0.2) - 1)) * (1 + 0.3) - 1) = 0.716", result.Calculation);
        }

        [Fact]
        public void AnnualizeByMonthShouldModifyValueAsExpected()
        {
            var financialMath = new FinancialMath();

            var result = financialMath.AnnualizeByMonth(0.716m, 13);

            Assert.Equal(0.646180649989542m, result.Value);
            Assert.Equal("((1 + 0.716) ^ (12 * 1 / 13)) - 1 = 0.646180649989542", result.Calculation);
        }
    }
}