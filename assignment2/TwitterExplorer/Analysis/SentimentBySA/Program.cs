using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;
using VaderExtended;

namespace SentimentBySA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var analyzer = new SentimentIntensityAnalyzer();
            Console.WriteLine($"Start {DateTime.Now}");

            var sad = new SADictionary();

            // location feature sets


            const string src =
                @"C:\Users\apk\ccviews\uni\TwitterExplore\Extracts\ExtractAll\bin\twitter-geotagged-posters.json";

            var geoPosts = new JsonRead<TagPosterDetails>(src);
            geoPosts.DoLoad();


            var ratings = new List<Tuple<double, string, StatisticalAreaClassification>>();
            foreach (var post in geoPosts.Records)
            {
                // find areas
                if (!post.Xloc.HasValue || !post.Yloc.HasValue) continue;

                var pt = new LatLong(post.Xloc.Value, post.Yloc.Value);
                var regions = sad.WhatRegions(pt);
                var res = analyzer.PolarityScores(post.Text);

                ratings.Add(new Tuple<double, string, StatisticalAreaClassification>(
                    res.Compound, post.Text, regions));
            }

            // collates stats & output
            foreach (var sa in new[] {StatArea.SA1, StatArea.SA2, StatArea.SA3, StatArea.SA4})
                using (var of = new StreamWriter($@"..\..\SentimentByRegion-{sa}.csv"))
                {
                    of.WriteLine("RegionId,Name,Observations,Sentiment");

                    // collate regional averages
                    foreach (var rec in ratings
                        .Where(x => x.Item3.Regions.ContainsKey(sa))
                        .Select(x => new {Loc = x.Item3.Regions[sa], Sentiment = x.Item1})
                        .GroupBy(x => x.Loc).ToList())
                    {
                        var count = rec.Count();
                        var avg = rec.Average(x => x.Sentiment);

                        of.WriteLine($"{rec.Key.Id}\t{rec.Key.Name}\t{count}\t{avg:F3}");
                    }
                }
        }
    }
}