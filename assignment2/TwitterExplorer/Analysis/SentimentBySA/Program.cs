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
        private const string loc = @"A:\aurin";


        private static void Main(string[] args)
        {
            var analyzer = new SentimentIntensityAnalyzer();
            Console.WriteLine($"Start {DateTime.Now}");

            var sad = new SADictionary();

            const string xmlTemplate = @"medians-{1}p02.xml";
            var cfg = new[] {StatArea.SA4, StatArea.SA3, StatArea.SA2, StatArea.SA1};


            // location feature sets
            var saLoader = new LoadStatisticalAreas();
            foreach (var area in cfg)
            {
                var xmlFile = Path.Combine(loc, string.Format(xmlTemplate, loc, area.ToString().ToLower()));
                var features = saLoader.GetFeatures(xmlFile);
                sad.SASets.Add(area, features);
            }


            var dataSrc = "twitter-extract-all.json";

            var geoPosts = new JsonRead<TagPosterDetails>(Path.Combine(loc, dataSrc));
            geoPosts.DoLoad();


            var ratings = new List<Tuple<double, string, StatisticalAreaClassification>>();
            foreach (var post in geoPosts.Records)
            {
                // find areas
                if (!post.Xloc.HasValue || !post.Yloc.HasValue) continue;

                var pt = new LatLong(post.Xloc.Value, post.Yloc.Value);
                var regions = sad.WhatRegions(pt);
                var res = analyzer.PolarityScores(post.Text);

                // NEED TIME OF DAY ASWELL 

                ratings.Add(new Tuple<double, string, StatisticalAreaClassification>(
                    res.Compound, post.Text, regions));
            }

            // collates stats & output
            foreach (var sa in cfg)
            {
                var clusteredBySa = ratings
                    .Where(x => x.Item3.Regions.ContainsKey(sa))
                    .Select(x => new KeyValuePair<long, double>(x.Item3.Regions[sa].Id, x.Item1))
                    .ToLookup(x => x.Key);

                using (var of = new StreamWriter($@"..\..\SentimentByRegion-{sa}.csv"))
                {
                    of.WriteLine("RegionId,Name,Observations,Sentiment");

                    // collate regional averages
                    foreach (var rec in clusteredBySa)
                    {
                        var count = rec.Count();
                        var avg = rec.Average(x => x.Value);

                        of.WriteLine($"{rec.Key}\t{sad.SANames[sa][rec.Key]}\t{count}\t{avg:F3}");
                    }
                }
            }
        }
    }
}