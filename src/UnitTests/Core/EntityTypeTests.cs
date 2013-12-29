using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class EntityTypeTests
    {
        [Fact]
        public void FromCodeShouldReturnEntityTypeWhenGivenMatchingCode()
        {
            var entityType = EntityType.FromCode(EntityType.Portfolio.Code);
            Assert.Equal(entityType, EntityType.Portfolio);
        }

        [Fact]
        public void FromCodeShouldReturnNullWhenGivenNonMatchingCode()
        {
            var entityType = EntityType.FromCode('3');
            Assert.Equal(entityType, null);
        }
    }
}