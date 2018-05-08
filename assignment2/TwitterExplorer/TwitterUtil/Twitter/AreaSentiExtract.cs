using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Transactions;

namespace TwitterUtil.Twitter
{
    [DataContract]
    public class AreaSentiExtract
    {
        [DataMember(Name = "city")] public string City { get; set; }
        [DataMember(Name = "country")] public string Country { get; set; }
        [DataMember(Name = "created_at")] public string CreatedAt { get; set; }
        [DataMember(Name = "day")] public string Day { get; set; }
        [DataMember(Name = "geo")] public List<double> Geo { get; set; }
        [DataMember(Name = "hour")] public int Hour { get; set; }
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "lang")] public string Lang { get; set; }
        [DataMember(Name = "month")] public string Month { get; set; }
        [DataMember(Name = "sentiment")] public double Sentiment { get; set; }
        [DataMember(Name = "text")] public string Text { get; set; }
        [DataMember(Name = "user")] public long User { get; set; }


        public double Xloc => Geo[1];
        public double Yloc => Geo[0];
    }
}