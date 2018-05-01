using System;

namespace TwitterUtil.Geo
{
    public class StatAreaLocation : IComparable<StatAreaLocation>, IEquatable<StatAreaLocation>
    {
        public StatAreaLocation(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }


        public int CompareTo(StatAreaLocation other) => Id.CompareTo(other.Id);
        public bool Equals(StatAreaLocation other) => Id == other.Id;
    }
}