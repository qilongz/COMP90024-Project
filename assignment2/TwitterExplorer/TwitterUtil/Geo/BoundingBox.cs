using System;
using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [DataContract]
    public class BoundingBox
    {
        public const double Tolerance = 100; // +/- 100 metres

        public BoundingBox(double latitude, double longitude)
        {
            var coord = new GeoCoordinate(latitude, longitude);

            var lowerLeft = coord.CalculateDerivedPosition(Tolerance * Math.Sqrt(2), -135);
            var upperRight = coord.CalculateDerivedPosition(Tolerance * Math.Sqrt(2), 45);

            Xmin = Math.Min(lowerLeft.Longitude, upperRight.Longitude);
            Xmax = Math.Max(lowerLeft.Longitude, upperRight.Longitude);
            Ymin = Math.Min(lowerLeft.Latitude, upperRight.Latitude);
            Ymax = Math.Max(lowerLeft.Latitude, upperRight.Latitude);
        }


        public BoundingBox(string sequence)
        {
            var arr = sequence.Split(' ');
            if (arr.Length != 2) throw new ArgumentException("missing");

            var lowerLeft = new LatLong(arr[0]);
            var upperRight = new LatLong(arr[1]);

            Xmin = Math.Min(lowerLeft.Lon, upperRight.Lon);
            Xmax = Math.Max(lowerLeft.Lon, upperRight.Lon);
            Ymin = Math.Min(lowerLeft.Lat, upperRight.Lat);
            Ymax = Math.Max(lowerLeft.Lat, upperRight.Lat);
        }


        public BoundingBox()
        {
        }

        [DataMember] public double Xmin { get; private set; }
        [DataMember] public double Xmax { get; private set; }
        [DataMember] public double Ymin { get; private set; }
        [DataMember] public double Ymax { get; private set; }

        public bool InBox(double latitude, double longitude)
        {
            return Xmin <= longitude && longitude <= Xmax &&
                   Ymin <= latitude && latitude <= Ymax;
        }


        public bool InBox(LatLong pt)
        {
            return Xmin <= pt.Lon && pt.Lon <= Xmax &&
                   Ymin <= pt.Lat && pt.Lat <= Ymax;
        }
    }
}