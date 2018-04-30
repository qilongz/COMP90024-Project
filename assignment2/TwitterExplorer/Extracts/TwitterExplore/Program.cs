using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TwitterUtil;
using TwitterUtil.TweetSummary;

namespace TwitterExplore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} filter");
                return;
            }

            //  GetDetailsFromTwitterJson(args[0]);

            GetDetailsFromTwitterJson("none", 100000000);
        }


        private static void GetDetailsFromTwitterJson(string filter, int size)
        {
            var tgtLocs = new List<string>
            {
                //  $@"E:\uni\Cluster and Cloud Computing\twitter-{filter}"
                @"A:\twitter"
               // @"A:\tmp"
            };

            var jr = new JsonRead(tgtLocs, false, size);    // {SingleThreaded = true};

            jr.DoLoad();


            // extract tags
            var tags = jr.Records
                .Where(x => x.HashTags != null)
                .SelectMany(x => x.HashTags.Select(t => t.ToLower()))
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            using (var ofs = new StreamWriter($@"..\..\activeTags-{filter}.csv"))
            {
                ofs.WriteLine("Tag,Count");
                foreach (var kvp in tags.OrderByDescending(x => x.Value)) ofs.WriteLine($"\"{kvp.Key}\",{kvp.Value}");
            }


            // extract timing
            var timing = jr.Records
                .GroupBy(x => new {x.CreateTime.Year, x.CreateTime.DayOfYear})
                .ToDictionary(x => x.Key, x => x.Count());

            using (var ofs = new StreamWriter($@"..\..\calendarActivity-{filter}.csv"))
            {
                ofs.WriteLine("Year,DayOfYear,Count");
                foreach (var kvp in timing.OrderByDescending(x => x.Value))
                    ofs.WriteLine($"{kvp.Key.Year},{kvp.Key.DayOfYear},{kvp.Value}");
            }

            // extract timing
            var locs = jr.Records
                .GroupBy(x => new {x.Yloc, x.Xloc})
                .ToDictionary(x => x.Key, x => x.Count());

            using (var ofs = new StreamWriter($@"..\..\location-{filter}.csv"))
            {
                ofs.WriteLine("Yloc,Xloc,Count");
                foreach (var kvp in locs.OrderByDescending(x => x.Value))
                    ofs.WriteLine($"{kvp.Key.Yloc},{kvp.Key.Xloc},{kvp.Value}");
            }


            //  jr.DumpToFile($@"..\..\twitter-geoOnly-{filter}.csv", true);
            //  jr.DumpToFile($@"..\..\twitter-extract-{filter}.csv");


            var collate = jr.Records.GroupBy(x => x.UserIdStr).ToDictionary(x => x.Key, x => x.ToList());

            using (var ofs = new StreamWriter($@"..\..\twitter-extract-{filter}.csv"))
            {
                ofs.WriteLine(TagPosterDetails.GetHeader());

                foreach (var kvp in collate.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key))
                foreach (var item in kvp.Value.OrderBy(x => x.CreateTime).ThenBy(x => x.Location))
                {
                    item.Count = kvp.Value.Count;
                    ofs.WriteLine(item.ToString());
                }
            }

            var nl = (byte) '\n';

            var ser = new DataContractJsonSerializer(typeof(TagPosterDetails));
            using (var fs = File.Open($@"..\..\twitter-extract-{filter}.json", FileMode.Create))
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