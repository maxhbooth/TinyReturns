using Headspring;

namespace Dimensional.TinyReturns.Core
{
    public class EntityType : Enumeration<EntityType>
    {
        public static EntityType Portfolio = new EntityType(1, "Portfolio", 'P');
        public static EntityType Benchmark = new EntityType(2, "Benchmark", 'B');

        private readonly char _code;

        public EntityType(
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
    }
}