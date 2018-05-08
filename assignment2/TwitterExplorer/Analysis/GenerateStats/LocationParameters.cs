using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TwitterUtil.Extraction;
using TwitterUtil.TweetSummary;

namespace GenerateStats
{
    [DataContract]
    public class LocationParameters
    {
        [DataMember] public string PostId { get; set; }
        [DataMember] public string Location { get; set; }
        [DataMember] public DateTime LocalTime { get; set; }
        [DataMember] public long UserId { get; set; }
        [DataMember] public bool HasPlace { get; set; }
        [DataMember] public string PlaceName { get; set; }
        [DataMember] public bool GeoEnabled { get; set; }
        [DataMember] public double? Yloc { get; set; }
        [DataMember] public double? Xloc { get; set; }
    }


    public class LocationParametersExtractor : IExtractor<LocatedTweet, LocationParameters>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public LocationParameters Extract(LocatedTweet src)
        {
            return new LocationParameters
            {
                PostId = src.PostId,
                Location = Get(_uniqueLocation, src.Location),
                LocalTime = src.LocalTime,

                HasPlace = src.HasPlace,
                PlaceName = Get(_uniquePlaceName, src.PlaceName),

                GeoEnabled = src.GeoEnabled,
                Yloc = src.Yloc,
                Xloc = src.Xloc,

                UserId = long.Parse(src.UserIdStr)
            };
        }

        public bool IsGeo(LocatedTweet src) => src.GeoEnabled;

        public string Get(HashSet<string> container, string item)
        {
            if (container.TryGetValue(item, out var master)) return master;

            container.Add(item);
            return item;
        }
    }


    public class ReloadLocationParameters : IExtractor<LocationParameters, LocationParameters>
    {
        private readonly HashSet<string> _uniqueLocation = new HashSet<string>();
        private readonly HashSet<string> _uniquePlaceName = new HashSet<string>();


        public LocationParameters Extract(LocationParameters src)
        {
            src.PostId = null; // clear to save space
            src.Location = Get(_uniqueLocation, src.Location);
            src.PlaceName = Get(_uniquePlaceName, src.PlaceName);

            return src;
        }

        public bool IsGeo(LocationParameters src) => src.GeoEnabled;
       

        public string Get(HashSet<string> container, string item)
        {
            if (container.TryGetValue(item, out var master)) return master;

            container.Add(item);
            return item;
        }
    }
}