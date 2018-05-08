using System;
using System.IO;
using System.Linq;
using FilterHospitals.Hospital;
using TwitterUtil;
using TwitterUtil.TweetSummary;
using TwitterUtil.Util;

namespace FilterHospitals
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string hospitals = @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Data\hospital.json";
            const string tweets =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\TwitterExplore\bin\twitter-extract-all.json";

            // load file criteria
            var eh = EmergencyHospitals.Load(hospitals);
            var tgts = new TargetRegions(eh.Features.Select(x => x.Description).ToList());

            var jr = new JsonRead<TagPosterDetails>(new[]{tweets});
            jr.DoLoad();
            Console.WriteLine("\n\n");


            using (var ofs = new StreamWriter(@"..\..\hospitalTags.csv"))
            {
                ofs.WriteLine($"TimeStamp,HospitalName,Suburb,State,Tags,Tweet");

                foreach (var tweet in jr.Records)
                    if (tgts.Find(tweet, out var hos))
                        ofs.WriteLine(
                            $"{tweet.CreateTime:s},{hos.Description.HospitalName.Pack()},{hos.Description.Suburb.Pack()}," +
                            $"{hos.Description.State.Pack()},{tweet.Tags},{tweet.Text.Pack()}");
            }
        }
    }
}