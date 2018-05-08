using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TwitterUtil.Util;

namespace TwitterUtil.TweetSummary
{
    [DataContract]
    [KnownType(typeof(TweetScore))]
    [KnownType(typeof(LocatedTweet))]
    public class TagPosterDetails
    {
        public TagPosterDetails()
        {
        }

        public TagPosterDetails(IReadOnlyList<string> arr)
        {
            var i = 0;
            Location = arr[i++];
            PostId = arr[i++];
            CreateTime = DateTime.Parse(arr[i++]);
            TimeZone = arr[i++];
            GeoEnabled = arr[i++][0] == 'T';
            Yloc = double.Parse(arr[i++]);
            Xloc = double.Parse(arr[i++]);
            UserIdStr = arr[i++];
            UserName = arr[i++];
            Count = int.Parse(arr[i++]);
            if (!string.IsNullOrWhiteSpace(arr[i])) HashTags = arr[i++].Split('#').ToList();
            Text = arr[i++];
        }

        [DataMember(Order = 1)] public string Location { get; set; }

        public DateTime CreateTime { get; set; }

        [DataMember(Order = 2)]
        public string Stamp
        {
            get => CreateTime.ToString("s");
            set => CreateTime = DateTime.Parse(value);
        }

        [DataMember(Order = 3)] public string PostId { get; set; }
        [DataMember(Order = 4)] public string TimeZone { get; set; }
        [DataMember(Order = 5)] public bool GeoEnabled { get; set; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public double? Yloc { get; set; } // == Geo[0] == Coordinates[1]

        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public double? Xloc { get; set; } // == Geo[1] == Coordinates[0]

        // user   
        [DataMember(Order = 8)] public string UserIdStr { get; set; }
        [DataMember(Order = 9)] public string UserName { get; set; }
        [DataMember(Order = 10)] public string Source { get; set; }

        // post   
        [DataMember(Order = 11, IsRequired = false, EmitDefaultValue = false)]
        public List<string> HashTags { get; set; }

        [DataMember(Order = 12)] public string Text { get; set; }
        [DataMember(Order = 13)] public string File { get; set; }
        [DataMember(Order = 14)] public int UtcOffset { get; set; }
        [DataMember(Order = 15)] public string ExpandedUrl { get; set; }


        public long RecordId { get; set; }
        public int Count { get; set; }


        public string Tags => HashTags == null ? "" : string.Join("#", HashTags).Pack();


        public DateTime LocalTime => CreateTime.AddSeconds(UtcOffset);

        public static string GetHeader() =>
            "Location,PostId,CreateTime,TimeZone,GeoEnabled,Yloc,Xloc,UserId,UserName,Count,HashTags,Text";

        public override string ToString()
        {
            return string.Join(",",
                Location,
                PostId,
                CreateTime.ToString("s"),
                TimeZone,
                GeoEnabled ? "\"T\"" : "\"F\"",
                Yloc, Xloc,
                UserIdStr.Pack(),
                UserName.Pack(),
                Count,
                Tags,
                Text.Pack()
            );
        }
    }
}