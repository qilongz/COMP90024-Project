using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterUtil.TweetSummary;
using TwitterUtil.Util;
using VaderExtended;

namespace SentimentBaseline
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var analyzer = new SentimentIntensityAnalyzer();
            Console.WriteLine($"Start {DateTime.Now}");

            const string src =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Extracts\FilteredExtract\bin\twitter-geotagged-posters.json";
            const string bigSrc =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Extracts\FilteredExtract\bin\data\twitter-all-geotagged-posters.json";

            var geoPosts = new JsonRead<TagPosterDetails>(new[]{bigSrc});
            geoPosts.DoLoad();

            var outcome = new List<KeyValuePair<double, string>>();
            foreach (var txt in geoPosts.Records.Select(x => x.Text))
            {
                var res = analyzer.PolarityScores(txt);
                outcome.Add(new KeyValuePair<double, string>(res.Compound, txt));
            }

            Console.WriteLine($"\nTotal Number: {outcome.Count:N0}\n");
            Console.WriteLine($"\nMin {outcome.Min(x => x.Key):P2}");
            Console.WriteLine($"Max {outcome.Max(x => x.Key):P2}");
            Console.WriteLine($"Number Zeros {outcome.Count(x => -0.02 < x.Key && x.Key < 0.02):N0}");
            Console.WriteLine($"Average rating {outcome.Average(x => x.Key):P2}\n");


            using (var of = new StreamWriter(@"..\..\BigScore.csv"))
            {
                foreach (var kvp in outcome.OrderByDescending(x => x.Key))
                    of.WriteLine($"{kvp.Key:P}\t{kvp.Value.Pack()}");
            }
        }
    }
}