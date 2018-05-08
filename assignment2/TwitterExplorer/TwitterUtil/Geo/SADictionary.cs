using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterUtil.Geo
{
    public class SADictionary
    {
        public SADictionary(Dictionary<StatArea, Features> srcFeatures)
        {
            SASets = new Dictionary<StatArea, OrderedFeatures>(srcFeatures.ToDictionary(x => x.Key,
                x => new OrderedFeatures(x.Value)));
        }

        public SADictionary(Dictionary<StatArea, OrderedFeatures> orderedFeatures)
        {
            SASets = orderedFeatures;
        }


        public Dictionary<StatArea, OrderedFeatures> SASets { get; }

        public Dictionary<StatArea, Dictionary<long, string>> SANames { get; } =
            new Dictionary<StatArea, Dictionary<long, string>>();

        public Dictionary<LatLong, StatisticalAreaClassification> Locations { get; } =
            new Dictionary<LatLong, StatisticalAreaClassification>();


        public void Merge(SADictionary src)
        {
            foreach (var kvp in src.SANames)
            {
                if (!SANames.TryGetValue(kvp.Key, out var outer))
                {
                    outer = new Dictionary<long, string>();
                    SANames.Add(kvp.Key, outer);
                }

                foreach (var ivp in kvp.Value)
                    outer[ivp.Key] = ivp.Value;
            }

            foreach (var kvp in src.Locations)
                if (!Locations.ContainsKey(kvp.Key))
                    Locations.Add(kvp.Key, new StatisticalAreaClassification(kvp.Value));
        }


        public StatisticalAreaClassification WhatRegions(LatLong pt)
        {
            if (Locations.TryGetValue(pt, out var area))
                return area;


            // else work out where fits
            var clas = new StatisticalAreaClassification(pt);
            foreach (var setpair in SASets)
            {
                (bool found, StatAreaLocation loc) = InFeatureSet(pt, setpair.Value);

                if (!found) (found, loc) = NearestBoundingBox(pt, setpair.Value);

                if (found)
                {
                    clas.Regions.Add(setpair.Key, loc);

                    if (!SANames.TryGetValue(setpair.Key, out var outer))
                    {
                        outer = new Dictionary<long, string>();
                        SANames.Add(setpair.Key, outer);
                    }

                    if (!outer.ContainsKey(loc.Id)) outer.Add(loc.Id, loc.Name);
                }
                else
                {
                 //   Console.WriteLine($"Still Missed {setpair.Key}\t{pt.Lat}, {pt.Lon}");
                }
            }

            return clas;
        }


        private (bool, StatAreaLocation) InFeatureSet(LatLong pt, OrderedFeatures locations)
        {
            foreach (var feat in locations.BoundariesContainingPoint(pt))
            foreach (var poly in feat.Locations)
                if (poly.PointInPolygon(pt))
                {
                    // allocate to the first region it finds

                    var areaName = feat.Parameters.Name;
                    var statisticalArea = long.Parse(feat.Parameters.Id);

                    return (true, new StatAreaLocation(statisticalArea, areaName));
                }


            return (false, StatAreaLocation.Null());
        }

        private (bool, StatAreaLocation) NearestBoundingBox(LatLong pt, OrderedFeatures locations)
        {
            foreach (var feat in locations.BoundariesContainingPoint(pt))
            {
                // allocate to the first bounding box  it finds

                var areaName = feat.Parameters.Name;
                var statisticalArea = long.Parse(feat.Parameters.Id);

                return (true, new StatAreaLocation(statisticalArea, areaName));
            }


            return (false, StatAreaLocation.Null());
        }
    }
}