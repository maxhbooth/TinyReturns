using System.Collections.Generic;
using System.Linq;

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

        public void AddReturnSeries(
            ReturnSeries a)
        {
            _returnSeries.Add(a);
        }

        public ReturnSeries[] GetAllReturnSeries()
        {
            return _returnSeries.ToArray();
        }

        public int ReturnSeriesCount
        {
            get { return _returnSeries.Count; }
        }

        // ** Equality

        protected bool Equals(Entity other)
        {
            if (!_returnSeries.SequenceEqual(other._returnSeries))
                return false;

            return EntityNumber == other.EntityNumber && string.Equals(Name, other.Name) && Equals(EntityType, other.EntityType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityNumber;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityType != null ? EntityType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }

    }
}