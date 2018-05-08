using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwitterUtil.TweetSummary
{
    [DataContract]
    public class LocatedTweet : TagPosterDetails
    {
        [DataMember(Order = 16)] public bool HasPlace { get; set; }
        [DataMember(Order = 17)] public string PlaceId { get; set; }
        [DataMember(Order = 18)] public string PlaceType { get; set; }
        [DataMember(Order = 19)] public string PlaceName { get; set; }

        [DataMember(Name = "Pos", Order = 20)] public List<double[]> BoxCoordinates { get; set; }
    }
}