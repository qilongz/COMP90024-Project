using System.Runtime.Serialization;

namespace TwitterUtil.TweetSummary
{
    [DataContract]
    public class TweetScore : TagPosterDetails
    {
        [DataMember(Name = "Neg", Order = 16)] public double Negative { get; set; }
        [DataMember(Name = "Neu", Order = 17)] public double Neutral { get; set; }
        [DataMember(Name = "Pos", Order = 18)] public double Positive { get; set; }
        [DataMember(Order = 19)] public double Compound { get; set; }
    }
}