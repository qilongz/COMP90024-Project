using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [CollectionDataContract]
    public class Features : List<Feature>
    {
    }


    public class OrderedFeatures
    {
        private readonly SortedList<double, List<Feature>> _list;
        private readonly List<double> _lons;

        // save some processing time as ordered by in longitude

        public OrderedFeatures(IEnumerable<Feature> src)
        {
            _list = new SortedList<double, List<Feature>>(src.GroupBy(x => x.BoundedBy.Xmax)
                .ToDictionary(x => x.Key, x => x.ToList()));
            _lons = _list.Keys.ToList();
        }

        public IEnumerable<Feature> BoundariesContainingPoint(LatLong pt)
        {
            // always step backwards to ensure start to the left of the possible boundary
            var offset = _lons.BinarySearch(pt.Lon);
            if (offset < 0) offset = ~offset - 1;
            if (offset < 0) offset = 0;


            // jump to nearest guess, then linearly scan all remainder

            for (; offset < _lons.Count; offset++)
            {
                var series = _list.Values[offset];
                foreach (var feat in series)
                    if (feat.BoundedBy.InBox(pt))
                        yield return feat;
            }
        }
    }


    [DataContract]
    public class Feature
    {
        [DataMember] public BoundingBox BoundedBy { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string BoundaryDescription { get; set; }

        [DataMember] public Aurin Parameters { get; set; }
        [DataMember] public Polygons Locations { get; set; } = new Polygons();

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public PolygonDescription LocationDescription { get; set; }


        public void Transform()
        {
            BoundedBy = new BoundingBox(BoundaryDescription);
            Locations = new Polygons(LocationDescription.Select(x => new Polygon(x)));
        }
    }


    [DataContract]
    public class Aurin
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public double Income { get; set; }
    }

    [CollectionDataContract]
    public class Polygons : List<Polygon>
    {
        public Polygons()
        {
        }

        public Polygons(IEnumerable<Polygon> src) : base(src)
        {
        }
    }

    [CollectionDataContract(ItemName = "Polygon")]
    public class PolygonDescription : List<string>
    {
    }
}