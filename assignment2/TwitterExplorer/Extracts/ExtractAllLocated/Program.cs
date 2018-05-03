using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using TwitterUtil.TweetSummary;

namespace ExtractAllLocated
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Start {DateTime.Now}");


            var srcLocs = new List<string> { @"A:\twitter" };
            var tgtLoc = @"A:\target";

            var jr = new LocatedJsonRead(srcLocs, 1000000) {SingleThreaded = false};
            jr.DoLoad();

            const byte nl = (byte) '\n';

            var ser = new DataContractJsonSerializer(typeof(LocatedTweet));
            using (var fs = File.Open(Path.Combine(tgtLoc, "twitter-geolocated.json"), FileMode.Create))
            {
                foreach (var rec in jr.Records)
                {
                    ser.WriteObject(fs, rec);
                    fs.WriteByte(nl);
                }
            }

            Console.WriteLine($"Done {DateTime.Now}");
        }
    }
}