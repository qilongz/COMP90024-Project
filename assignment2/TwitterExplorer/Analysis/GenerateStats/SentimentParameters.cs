using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TwitterUtil.Extraction;
using TwitterUtil.TweetSummary;

namespace GenerateStats
{
    [DataContract]
    [KnownType(typeof(GeoSentimentParameters))]
    [KnownType(typeof(SentimentParameters))]
    [KnownType(typeof(UserGeoSentimentParameters))]
    public class SentimentParametersBase
    {
        [DataMember(Name = "Loc")] public string Location { get; set; }
        [DataMember(Name = "When")] public DateTime LocalTime { get; set; }
        [DataMember(Name = "Uid")] public long UserId { get; set; }
        [DataMember(Name = "Place")] public string PlaceName { get; set; }
        [DataMember(Name = "Geo")] public bool GeoEnabled { get; set; }

        [DataMember(Name = "Score")] public double Compound { get; set; }
    }

    [DataContract]
    public class SentimentParameters : SentimentParametersBase
    {
        [DataMember(Name = "Pid")] public string PostId { get; set; }
    }


    public class SentimentExtractor : IExtractor<TweetScore, SentimentParameters>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public SentimentParameters Extract(TweetScore src)
        {
            return new SentimentParameters
            {
                PostId = src.PostId,
                Location = Get(_uniqueLocation, src.Location),
                LocalTime = src.LocalTime,

                PlaceName = Get(_uniquePlaceName, src.PlaceName),

                GeoEnabled = src.GeoEnabled,
                UserId = long.Parse(src.UserIdStr),

                Compound = src.Compound
            };
        }

        public bool IsGeo(TweetScore src) => src.GeoEnabled;
       

        public string Get(HashSet<string> container, string item)
        {
            if (container.TryGetValue(item, out var master)) return master;

            container.Add(item);
            return item;
        }
    }


    public class ReloadSentimentParameters : IExtractor<SentimentParametersBase, SentimentParametersBase>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public SentimentParametersBase Extract(SentimentParametersBase src)
        {
            src.Location = Get(_uniqueLocation, src.Location);
            src.PlaceName = Get(_uniquePlaceName, src.PlaceName);

            return src;
        }

        public bool IsGeo(SentimentParametersBase src) => src.GeoEnabled;
       

        public string Get(HashSet<string> container, string item)
        {
            if (container.TryGetValue(item, out var master)) return master;

            container.Add(item);
            return item;
        }
    }
}