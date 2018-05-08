using System;
using TwitterUtil.Geo;
using TwitterUtil.Twitter;

namespace GenerateStats
{
    public struct LocatedAreaScore
    {
        public AreaSentiExtract Parameters { get; set; }
        public StatisticalAreaClassification Area { get; }
        

        public LocatedAreaScore(AreaSentiExtract post, StatisticalAreaClassification area)
        {
            Parameters = post;
            Area = area;
           
        }
    }
}