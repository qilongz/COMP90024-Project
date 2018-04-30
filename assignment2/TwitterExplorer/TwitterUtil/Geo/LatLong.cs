using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [DataContract]
    public class LatLong
    {
        public LatLong()
        {
        }

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

        [DataMember] public double Lat { get; private set; }
        [DataMember] public double Lon { get; private set; }

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