using System;
using TwitterUtil.Geo;

namespace GenerateStats
{
    public struct LocatedScore
    {
        public GeoSentimentParameters Parameters { get; set; }
        public StatisticalAreaClassification Area { get; }
        

        public LocatedScore(GeoSentimentParameters post, StatisticalAreaClassification area)
        {
            Parameters = post;
            Area = area;
           
        }
    }
}