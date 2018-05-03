using System.Runtime.Serialization;

namespace TwitterUtil.TweetSummary
{
    [DataContract]
    public class TweetScore : TagPosterDetails
    {
        //[DataMember(Name = "SA", Order = 23)] public string StatisticalArea { get; set; }
        //[DataMember(Order = 24)] public string AreaName { get; set; }

        [DataMember(Name = "Neg", Order = 25)] public double Negative { get; set; }
        [DataMember(Name = "Neu", Order = 26)] public double Neutral { get; set; }
        [DataMember(Name = "Pos", Order = 27)] public double Positive { get; set; }
        [DataMember(Order = 28)] public double Compound { get; set; }
    }
}