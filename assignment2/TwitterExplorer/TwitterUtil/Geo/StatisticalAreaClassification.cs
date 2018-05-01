using System.Collections.Generic;
using TwitterUtil.Util;

namespace TwitterUtil.Geo
{
    public class StatisticalAreaClassification
    {
        public StatisticalAreaClassification(LatLong pt)
        {
            Location = pt;
        }

        public LatLong Location { get; set; }
        public Dictionary<StatArea, StatAreaLocation> Regions { get; } = new Dictionary<StatArea, StatAreaLocation>();
    }


    public class SADictionary
    {
        public SADictionary(IEnumerable<KeyValuePair<string, Features>> sets)
        {
            foreach (var set in sets)
            {
                var sa = set.Key.ToEnum<StatArea>();
                SASets.Add(sa, set.Value);
            }
        }

        public Dictionary<StatArea, Features> SASets { get; set; } = new Dictionary<StatArea, Features>();

        public Dictionary<LatLong, StatisticalAreaClassification> Locations { get; } =
            new Dictionary<LatLong, StatisticalAreaClassification>();

        public StatisticalAreaClassification WhatRegions(LatLong pt)
        {
            if (Locations.TryGetValue(pt, out var area)) return area;


            // else work out where fits
            var clas = new StatisticalAreaClassification(pt);
            foreach (var setpair in SASets)
            {
                (bool found, StatAreaLocation loc) = InFeatureSet(pt, setpair.Value);

                if (found) clas.Regions.Add(setpair.Key, loc);
            }

            return clas;
        }


        private (bool, StatAreaLocation) InFeatureSet(LatLong pt, Features locations)
        {
            foreach (var feat in locations)
                if (feat.BoundedBy.InBox(pt))
                    foreach (var poly in feat.Locations)
                        if (poly.PointInPolygon(pt))
                        {
                            // allocate to the first region it finds

                            var areaName = feat.Parameters.Name;
                            var statisticalArea = feat.Parameters.Id;

                            return (true, new StatAreaLocation(areaName, statisticalArea));
                        }

            return (false, null);
        }
    }

    public class StatAreaLocation
    {
        public StatAreaLocation(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }


    public enum StatArea
    {
        SA1,
        SA2,
        SA3,
        SA4
    }
}