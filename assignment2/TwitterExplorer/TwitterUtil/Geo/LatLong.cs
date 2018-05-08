using System;
using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [DataContract]
    public struct LatLong : IEquatable<LatLong>, IComparable<LatLong>
    {
        public LatLong(double lon, double lat)
        {
            Lat = lat;
            Lon = lon;
        }

        public LatLong(string lonLat)
        {
            var sep = lonLat.IndexOf(',');
            Lon = double.Parse(lonLat.Substring(0, sep));
            Lat = double.Parse(lonLat.Substring(sep + 1));
        }

        [DataMember] public double Lat { get; }
        [DataMember] public double Lon { get; }

        public int CompareTo(LatLong other)
        {
            var res = Lat.CompareTo(other.Lat);
            if (res != 0) return res;

            return Lon.CompareTo(other.Lon);
        }

        public bool Equals(LatLong other)
        {
            return Lat.Equals(other.Lat) && Lon.Equals(other.Lon);
        }

        public override int GetHashCode()
        {
            return Lat.GetHashCode() ^ Lon.GetHashCode();
        }

        public int IsLeft(LatLong p1, LatLong p2)
        {
            var calc = (p1.Lon - Lon) * (p2.Lat - Lat)
                       - (p2.Lon - Lon) * (p1.Lat - Lat);

            if (calc > 0) return 1;
            if (calc < 0) return -1;

            return 0;
        }
    }
}