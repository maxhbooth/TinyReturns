using System.Collections.Generic;

namespace Dimensional.TinyReturns.Core
{
    public class Entity
    {
        private readonly List<ReturnSeries> _returnSeries;

        public Entity()
        {
            _returnSeries = new List<ReturnSeries>();
        }

        public int EntityNumber { get; set; }
        public string Name { get; set; }
        public EntityType EntityType { get; set; }

        public void AddReturnSeries(
            IEnumerable<ReturnSeries> a)
        {
            _returnSeries.AddRange(a);
        }

        public ReturnSeries[] GetAllReturnSeries()
        {
            return _returnSeries.ToArray();
        }

        public int ReturnSeriesCount
        {
            get { return _returnSeries.Count; }
        }
    }
}