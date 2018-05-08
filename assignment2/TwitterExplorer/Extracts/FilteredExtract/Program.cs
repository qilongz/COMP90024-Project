using System;
using System.Collections.Generic;
using System.Linq;
using TwitterUtil.TweetSummary;

namespace FilteredExtract
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Start {DateTime.Now}");

            const string activeUsers =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Extracts\FilteredExtract\data\twitter-extract-all.json";
            const string tgtFile = @"..\..\twitter-all-geotagged-posters.json";
            var tgtLocs = new List<string> {@"A:\twitter"};

           


            var geoPosts = new JsonRead<TagPosterDetails>(new[]{activeUsers});
            geoPosts.DoLoad();

            // extract unique userIds
            var ids = new HashSet<string>(geoPosts.Records.Select(x => x.UserIdStr));
            Console.WriteLine($"Have {ids.Count} posters\n");

            var jr = new FilterJsonRead(tgtLocs, geoPosts.Records.Count, ids);
            jr.ExtractAndSave(tgtFile);

            Console.WriteLine($"Done {DateTime.Now}");
        }
    }
}