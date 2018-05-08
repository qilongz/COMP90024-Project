using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [DataContract]
    public class Polygon
    {
        // The Winding Number approach

        // http://geomalgorithms.com/a03-_inclusion.html
        // https://stackoverflow.com/questions/924171/geo-fencing-point-inside-outside-polygon


        public Polygon()
        {
        }

        public Polygon(string sequence)
        {
            var arr = sequence.Split(' ');
            Points = new List<LatLong>(arr.Length);

            foreach (var item in arr) Points.Add(new LatLong(item));
        }

        [DataMember] public List<LatLong> Points { get; private set; }


        public bool PointInPolygon(LatLong p)
        {
            var wn = 0; // the winding number counter
            var n = Points.Count - 1;

            // where last entry of Points the same as first entry

            // loop through all edges of the polygon
            for (var i = 0; i < n; i++)
                // edge from V[i] to V[i+1]
                if (Points[i].Lat <= p.Lat)
                {
                    // start y <= P.y
                    if (Points[i + 1].Lat > p.Lat && // an upward crossing
                        Points[i].IsLeft(Points[i + 1], p) > 0) // P left of edge
                        ++wn; // have a valid up intersect
                }
                else
                {
                    // start y > P.y (no test needed)
                    if (Points[i + 1].Lat <= p.Lat && // a downward crossing
                        Points[i].IsLeft(Points[i + 1], p) < 0) // P right of edge
                        --wn; // have a valid down intersect
                }

            return wn != 0;
        }
    }
}