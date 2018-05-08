using System;
using System.IO;
using System.Runtime.Serialization.Json;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;

namespace AnotateWithAurin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //   const string aurinData = @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Aurin\sample-sa4p02.xml";
            const string aurinData = @"a:\aurin\medians-sa2p02.xml";
            const string xlst = @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Aurin\extract.xslt";

            var cm = CensusMedians.Extract(aurinData, xlst);
            Console.WriteLine(cm.Features.Count);

            cm.TransformFeatures();

            const string activeUsers =
                @"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\Extracts\FilteredExtract\data\twitter-extract-all.json";
            const string outFile = @"..\..\..\data\twitter-all-areaTagged.json";


            var geoPosts = new JsonRead<TagPosterDetails>(activeUsers);
            geoPosts.DoLoad();


            var cnt = 0;
            const byte nl = (byte) '\n';
            var ser = new DataContractJsonSerializer(typeof(TagPosterDetails));
            using (var fs = File.Open(outFile, FileMode.Create))
            {
                foreach (var rec in geoPosts.Records)
                foreach (var feat in cm.Features)
                    if (rec.Yloc.HasValue && rec.Xloc.HasValue && feat.BoundedBy.InBox(rec.Yloc.Value, rec.Xloc.Value))
                    {
                        var pt = new LatLong(rec.Xloc.Value, rec.Yloc.Value);

                        // in the broad region, double check if falls into a nominated polygon
                        foreach (var poly in feat.Locations)
                            if (poly.PointInPolygon(pt))
                            {
                                rec.AreaName = feat.Parameters.Name;
                                rec.StatisticalArea = feat.Parameters.Id;

                                ser.WriteObject(fs, rec);
                                fs.WriteByte(nl);

                                if (++cnt % 1000 == 0)
                                    Console.WriteLine($"{rec.Location,-20} {rec.AreaName}");
                            }
                    }
            }
        }
    }
}