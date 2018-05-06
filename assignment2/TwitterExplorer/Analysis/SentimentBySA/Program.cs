using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;

namespace SentimentBySA
{
    internal class Program
    {
        private const string Loc = @"..\..";


        private static void Main(string[] args)
        {
            Console.WriteLine($"Start {DateTime.Now}");

            var requiredUsers = new Dictionary<string, string>();
            using (var ifs = new StreamReader(@"..\..\userHomeCity.csv"))
            {
                var ln = ifs.ReadLine(); // skip header
                while ((ln = ifs.ReadLine()) != null)
                {
                    var arr = ln.Split(',');
                    requiredUsers.Add(arr[0],arr[1]);
                }
            }

                const string xmlTemplate = @"medians-{1}p02.xml";
            var cfg = new[] {StatArea.SA4, StatArea.SA3, StatArea.SA2, StatArea.SA1};


            // location feature sets
            var saLoader = new LoadStatisticalAreas();
            var featureSets = new Dictionary<StatArea, Features>();
            foreach (var area in cfg)
            {
                var xmlFile = Path.Combine(Loc, string.Format(xmlTemplate, Loc, area.ToString().ToLower()));
                var features = saLoader.GetFeatures(xmlFile);
                featureSets.Add(area, features);
            }

            // summarise
            foreach (var area in cfg)
            {
                Console.WriteLine($"{area}\tregions:{featureSets[area].Count,6:N0}\tploygons: {featureSets[area].Sum(x=>x.Locations.Count),8:N0}");
            }

            var sad = new SADictionary(featureSets);


            //      var dataSrc = "twitter-extract-all.json"; var geoPosts = new JsonRead<TagPosterDetails>(new[]{Path.Combine(Loc, dataSrc)});

            var geoPosts = new JsonRead<TagPosterDetails>(new[]{ @"A:\locatedTargets" });
            geoPosts.DoLoad();


            var filtered = geoPosts.Records.Where(x => requiredUsers.ContainsKey(x.UserIdStr)).ToList();


            var cls = new Classify(filtered, sad); // {SingleThreaded = true};
            cls.DoClassification();




            foreach (var sa in cfg)
            {
                var clusteredBySa = cls.Scores
                    .Where(x => x.Area.Regions.ContainsKey(sa))
                    .Select(x => new KeyValuePair<long, double>(x.Area.Regions[sa].Id, x.Score))
                    .ToLookup(x => x.Key);

                using (var of = new StreamWriter($@"..\..\SentimentFilterWithRegion-{sa}.csv"))
                {
                    of.WriteLine("RegionId,Name,Observations,Sentiment");

                    // collate regional averages
                    foreach (var rec in clusteredBySa)
                    {
                        var count = rec.Count();
                        var avg = rec.Average(x => x.Value);

                        of.WriteLine($"{rec.Key},\"{sad.SANames[sa][rec.Key]}\",{count},{avg:F4}");
                    }
                }
            }
        }
    }
}