using System.Runtime.Serialization;

namespace TwitterUtil.TweetSummary
{
    [DataContract]
    public class TweetScore : LocatedTweet
    {
        [DataMember(Order = 25)] public double Negative { get; set; }
        [DataMember(Order = 26)] public double Neutral { get; set; }
        [DataMember(Order = 27)] public double Positive { get; set; }
        [DataMember(Order = 28)] public double Compound { get; set; }
    }
}