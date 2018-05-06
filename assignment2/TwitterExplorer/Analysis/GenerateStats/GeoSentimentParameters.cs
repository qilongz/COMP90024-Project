using System.Collections.Generic;
using System.Runtime.Serialization;
using TwitterUtil.Extraction;
using TwitterUtil.TweetSummary;

namespace GenerateStats
{
    [DataContract]
    public class GeoSentimentParameters : SentimentParametersBase
    {
        [DataMember] public double Yloc { get; set; }
        [DataMember] public double Xloc { get; set; }
    }


    public class UserGeoSentimentParameters : GeoSentimentParameters
    {
        public string PostId { get; set; }
    }


    public class ReloadGeoSentimentParameters : IExtractor<GeoSentimentParameters, GeoSentimentParameters>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public GeoSentimentParameters Extract(GeoSentimentParameters src)
        {
            src.Location = Get(_uniqueLocation, src.Location);
            src.PlaceName = Get(_uniquePlaceName, src.PlaceName);

            return src;
        }

        public bool IsGeo(GeoSentimentParameters src) => true;

        public string Get(HashSet<string> container, string item)
        {
            if (container.TryGetValue(item, out var master)) return master;

            container.Add(item);
            return item;
        }
    }


    public class GeoSentimentExtractor : IExtractor<TweetScore, UserGeoSentimentParameters>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public UserGeoSentimentParameters Extract(TweetScore src)
        {
            return new UserGeoSentimentParameters
            {
                PostId = src.PostId,
                Location = Get(_uniqueLocation, src.Location),
                LocalTime = src.LocalTime,

                PlaceName = Get(_uniquePlaceName, src.PlaceName),

                GeoEnabled = src.GeoEnabled,
                UserId = long.Parse(src.UserIdStr),

                Compound = src.Compound,
                Xloc = src.Xloc.Value,
                Yloc = src.Yloc.Value
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
}