using System;
using System.IO;
using System.Linq;
using TwitterUtil.TweetSummary;
using TwitterUtil.Util;

namespace MultiCities
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string tweets =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Extracts\FilteredExtract\data\twitter-extract-all.json";
            var jr = new JsonRead<TagPosterDetails>(new[]{tweets});
            jr.DoLoad();
            Console.WriteLine("\n\n");

            var collateByUserLocation = jr.Records
                .GroupBy(x => x.UserIdStr)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(u => u.Location)
                        .ToDictionary(u => u.Key, u => u.ToList()));

            const int freqFilter = 20;

            // those users who have posted from multiple cities
            var freqMultiCitiesPosters = collateByUserLocation
                .Where(x => x.Value.Count > 1)  // more than 1 city
                .Where(x=>x.Value.Any(c=>c.Value.Count>= freqFilter))     // more than 20 in any single city
                .ToDictionary(x => x.Key, x => x.Value.SelectMany(t => t.Value).ToList());

            using (var ofs = new StreamWriter($@"..\..\frequentMultiCitiesTweeters-{freqFilter}.csv"))
            {
                ofs.WriteLine($"UserId,UserName,Count,TimeStamp,Location,Yloc,Xloc,Tags,Tweet");

                foreach (var multi in freqMultiCitiesPosters.OrderByDescending(x => x.Value.Count))
                {
                    var id = multi.Key;
                    var name = multi.Value.First().UserName;
                    var cnt = multi.Value.Count;

                    foreach (var rec in multi.Value.OrderBy(x => x.CreateTime))
                        ofs.WriteLine(
                            $"{id},{name.Pack()},{cnt},{rec.CreateTime:s},{rec.Location}," +
                            $"{rec.Yloc},{rec.Xloc},{rec.Tags},{rec.Text.Pack()}");
                }
            }
        }
    }
}