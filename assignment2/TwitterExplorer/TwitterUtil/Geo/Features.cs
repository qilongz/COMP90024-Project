using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TwitterUtil.Geo
{
    [CollectionDataContract]
    public class Features : List<Feature>
    {
        
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