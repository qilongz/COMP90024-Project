using System;
using TwitterUtil.Geo;

namespace SentimentBySA
{
    public struct LocatedScore
    {
        public StatisticalAreaClassification Area { get; }
        public double Score { get; }
        public DateTime LocalTime { get; }

        public LocatedScore(double score, DateTime when, StatisticalAreaClassification area)
        {
            Score = score;
            Area = area;
            LocalTime = when;
        }
    }
}