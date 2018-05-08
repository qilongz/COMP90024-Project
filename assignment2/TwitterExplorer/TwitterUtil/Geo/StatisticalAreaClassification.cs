using System.Collections.Generic;

namespace TwitterUtil.Geo
{
    public class StatisticalAreaClassification
    {
        public StatisticalAreaClassification(LatLong pt)
        {
            Location = pt;
        }


        public StatisticalAreaClassification(StatisticalAreaClassification src)
        {
            Location = src.Location;
            Regions = new Dictionary<StatArea, StatAreaLocation>(src.Regions);
        }


        public LatLong Location { get; set; }
        public Dictionary<StatArea, StatAreaLocation> Regions { get; } = new Dictionary<StatArea, StatAreaLocation>();
    }
}