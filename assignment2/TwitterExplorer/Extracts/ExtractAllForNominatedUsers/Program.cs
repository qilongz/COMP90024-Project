using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TwitterUtil.TweetSummary;

namespace ExtractAllForNominatedUsers
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Start {DateTime.Now}");

            var geoPosts = new JsonRead<TagPosterDetails>(
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\TwitterExplore\bin\twitter-extract-all.json");
            geoPosts.DoLoad();

            // extract unique userIds
            var ids = new HashSet<string>(geoPosts.Records.Select(x => x.UserIdStr));
            Console.WriteLine($"Have {ids.Count} posters\n");


            var tgtLocs = new List<string> {@"A:\twitter"};
            var jr = new FilterJsonRead(tgtLocs, geoPosts.Records.Count, ids);
            jr.DoLoad();

            const byte nl = (byte) '\n';

            var ser = new DataContractJsonSerializer(typeof(TagPosterDetails));
            using (var fs = File.Open(@"..\..\twitter-geotagged-posters.json", FileMode.Create))
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