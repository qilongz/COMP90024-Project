using System.Collections.Generic;
using System.Runtime.Serialization;


/*
 * 
<wfs:FeatureCollection xmlns="http://www.opengis.net/wfs"
 xmlns:wfs="http://www.opengis.net/wfs"
 xmlns:gml="http://www.opengis.net/gml" 
 xmlns:aurin="http://www.aurin.org.au" 
 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"

 */


namespace TwitterUtil.Opengis
{
    [CollectionDataContract(Namespace = "http://www.opengis.net/wfs")]
    internal class FeatureCollection : List<FeatureMember>
    {
    }

    [DataContract(Name = "featureMember", Namespace = "http://www.opengis.net/gml")]
    public class FeatureMember
    {
        [DataMember(Name = "datasource-AU_Govt_ABS-UoM_AURIN_DB_2_sa4_p02_selected_medians_and_averages_census_2016")]
        public Datasource Data { get; set; }
    }

    [DataContract(Namespace = "http://www.aurin.org.au")]
    public class Datasource
    {
        [DataMember(Name = "boundedBy", Order = 1)]
        public BoundedBy By { get; set; }

        [DataMember(Name = "sa4_name16")] public string Name { get; set; }
    }

    [DataContract(Namespace = "http://www.aurin.org.au")]
    public class AurinString
    {
    }


    [DataContract(Name = "featureMember", Namespace = "http://www.opengis.net/gml")]
    public class BoundedBy
    {
    }
}