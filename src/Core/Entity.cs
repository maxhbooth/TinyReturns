namespace Dimensional.TinyReturns.Core
{
    public class Entity
    {
        public Entity()
        {
            ReturnSeries = new ReturnSeries[0];
        }

        public int EntityNumber { get; set; }
        public string Name { get; set; }
        public EntityType EntityType { get; set; }

        public ReturnSeries[] ReturnSeries { get; set; }
    }
}