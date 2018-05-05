using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TwitterUtil.TweetSummary;

namespace GenerateStats
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CollateSentiments();

            // Collate();

            //   var data = LoadAlreadyLocationsCollated();
            //   AnalyseLocations(data);

            //var data = LoadAlreadySentimentsCollated();
            //AnalyseSentiments(data);
        }


        public static void CollateLocations()
        {
            string[] tweets = {@"A:\withRateWithSentiment"};
            Console.WriteLine($"Start {DateTime.Now}");

            var eng = new BatchEngine<LocatedTweet, LocationParametersExtractor, LocationParameters>(8, tweets);
            eng.Process();
            Console.WriteLine($"Have {eng.Count,12:N0} records\n\n");


            const byte nl = (byte) '\n';

            var ser = new DataContractJsonSerializer(typeof(LocationParameters));
            using (var fs = File.Open(@"A:\locationParameters\series-LocationParameters.json", FileMode.Create))
            {
                var ids = new HashSet<string>();
                var cnt = 0;

                foreach (var item in eng.Records())
                {
                    if (ids.Contains(item.PostId)) continue;

                    ids.Add(item.PostId);
                    ser.WriteObject(fs, item);
                    fs.WriteByte(nl);

                    if (++cnt % 500000 == 0) Console.WriteLine($"Written {cnt,12:N0}");
                }
            }
        }

        public static void CollateSentiments()
        {
            string[] tweets = {@"A:\withRateWithSentiment"};
            Console.WriteLine($"Start {DateTime.Now}");

            var eng = new BatchEngine<TweetScore, SentimentExtractor, SentimentParameters>(8, tweets);
            eng.Process();
            Console.WriteLine($"Have {eng.Count,12:N0} records\n\n");


            const byte nl = (byte) '\n';

            var ser = new DataContractJsonSerializer(typeof(SentimentParameters));
            using (var fs = File.Open(@"A:\sentimentParameters\series-SentimentParameters.json", FileMode.Create))
            {
                var ids = new HashSet<string>();
                var cnt = 0;

                foreach (var item in eng.Records())
                {
                    if (ids.Contains(item.PostId)) continue;
                    ser.WriteObject(fs, item);
                    fs.WriteByte(nl);

                    if (++cnt % 500000 == 0) Console.WriteLine($"Written {cnt,12:N0}");
                }
            }
        }


        public static BatchEngine<LocationParameters, ReloadLocationParameters, LocationParameters> LoadAlreadyLocationsCollated()
        {
            string[] locations = {@"A:\locationParameters"};
            Console.WriteLine($"Start {DateTime.Now}");

            var eng = new BatchEngine<LocationParameters, ReloadLocationParameters, LocationParameters>(8, locations);
            eng.Process();

            Console.WriteLine($"Have {eng.Count,12:N0} records\n\n");
            return eng ;
        }

        public static BatchEngine<SentimentParametersBase, ReloadSentimentParameters, SentimentParametersBase> 
            LoadAlreadySentimentsCollated()
        {
            string[] locations = {@"A:\sentimentParameters"};
            Console.WriteLine($"Start {DateTime.Now}");

            var eng =new
                BatchEngine<SentimentParametersBase, ReloadSentimentParameters, SentimentParametersBase>
                (8,locations);
            eng.Process();

            Console.WriteLine($"Have {eng.Count,12:N0} records\n\n");
            return eng;
        }


        public static void AnalyseLocations(BatchEngine<LocationParameters, ReloadLocationParameters, LocationParameters> be)
        {
            Console.WriteLine($"Analysing \n");

            // ---------------------------------------------------------
            // collate the base statistics

            // by city, by year-month
            var collateByCity = be.Records().AsParallel()
                .GroupBy(x => x.Location)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(d => d.LocalTime.ToString("yyyy-MM-01"))
                        .ToDictionary(t => t.Key,
                            t => t.GroupBy(g => new {Place = g.HasPlace, Exact = g.GeoEnabled})
                                .ToDictionary(c => c.Key, c => c.Count())));

            var ti = CultureInfo.CurrentCulture.TextInfo;

            using (var ofs = new StreamWriter(@"..\..\volumeStats.csv"))
            {
                ofs.WriteLine($"Location,YearMth,HasPlace,HasGeo,Count");

                foreach (var kvp in collateByCity.OrderBy(x => x.Key))
                foreach (var vgp in kvp.Value.OrderBy(x => x.Key))
                foreach (var vcp in vgp.Value)
                    ofs.WriteLine(
                        $"{ti.ToTitleCase(kvp.Key)},{vgp.Key},{(vcp.Key.Place ? 'T' : 'F')},{(vcp.Key.Exact ? 'T' : 'F')},{vcp.Value}");
            }

            // ---------------------------------------------------------
            // extract unique locations

            var locs = be.Records().AsParallel()
                .Where(x => x.GeoEnabled && x.Yloc.HasValue && x.Xloc.HasValue)
                .GroupBy(x => new {Y = x.Yloc.Value, X = x.Xloc.Value})
                .ToDictionary(x => x.Key, x => x.Count());

            using (var ofs = new StreamWriter($@"..\..\locations.csv"))
            {
                ofs.WriteLine("Yloc,Xloc,Count");
                foreach (var kvp in locs.OrderByDescending(x => x.Value))
                    ofs.WriteLine($"{kvp.Key.Y},{kvp.Key.X},{kvp.Value}");
            }


            // ---------------------------------------------------------
            // extract time of day

            // by city, by year-month
            var timeOfDay = be.Records().AsParallel()
                .GroupBy(d => d.LocalTime.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.GroupBy(d => d.LocalTime.ToString("HH"))
                    .ToDictionary(d => d.Key, d => d.Count()));

            using (var ofs = new StreamWriter($@"..\..\timeOfDayActivity.csv"))
            {
                ofs.WriteLine("DayOfWeek,Hour,Count");
                foreach (var dvp in timeOfDay)
                foreach (var kvp in dvp.Value)
                    ofs.WriteLine($"{dvp.Key},{kvp.Key},{kvp.Value}");
            }


            // ---------------------------------------------------------
            // extract unique users

            // by city, by year-month
            var usersMth = be.Records().AsParallel()
                .Where(x => x.GeoEnabled)
                .GroupBy(d => d.LocalTime.ToString("yyyy-MM-01"))
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(u => u.UserId)
                        .ToDictionary(t => t.Key, t => t.Count()));

            using (var ofs = new StreamWriter($@"..\..\geoUsersPerMth.csv"))
            {
                ofs.WriteLine("YearMth,DistinctUsers");
                foreach (var kvp in usersMth)
                    ofs.WriteLine($"{kvp.Key},{kvp.Value.Count}");
            }


            // ---------------------------------------------------------

            var collateByUserLocation = be.Records().AsParallel()
                .GroupBy(x => x.UserId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(u => u.Location)
                        .ToDictionary(u => u.Key, u => u.ToList()));

            const int freqFilter = 20;

            // those users who have posted from multiple cities
            var freqMultiCitiesPosters = collateByUserLocation
                .Where(x => x.Value.Count > 1) // more than 1 city
                .Where(x => x.Value.Any(c => c.Value.Count >= freqFilter)) // more than 20 in any single city
                .ToDictionary(x => x.Key, x => x.Value.SelectMany(t => t.Value).ToList());

            using (var ofs = new StreamWriter($@"..\..\frequentMultiCitiesTweeters-{freqFilter}.csv"))
            {
                ofs.WriteLine($"UserId,Count,LocalTime,Location,Yloc,Xloc");

                foreach (var multi in freqMultiCitiesPosters.OrderByDescending(x => x.Value.Count))
                {
                    var id = multi.Key;
                    var cnt = multi.Value.Count;

                    foreach (var rec in multi.Value.OrderBy(x => x.LocalTime))
                        ofs.WriteLine(
                            $"{id},{cnt},{rec.Location:s},{rec.Location}," +
                            $"{rec.Yloc},{rec.Xloc}");
                }
            }
        }






        public static void AnalyseSentiments(BatchEngine<SentimentParametersBase, ReloadSentimentParameters, SentimentParametersBase> be)
        {
            Console.WriteLine($"Analysing \n");

            // ---------------------------------------------------------
            // collate the base statistics

            // by city, by year-month
            var collateByCity = be.Records().AsParallel()
                .GroupBy(x => x.Location)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(d => d.LocalTime.ToString("yyyy-MM-01"))
                        .ToDictionary(t => t.Key,
                            t => t.Average(i=>i.Compound)));

            var ti = CultureInfo.CurrentCulture.TextInfo;

            using (var ofs = new StreamWriter(@"..\..\sentimentStats.csv"))
            {
                ofs.WriteLine($"Location,YearMth,AverageSentiment");

                foreach (var kvp in collateByCity.OrderBy(x => x.Key))
                    foreach (var vgp in kvp.Value.OrderBy(x => x.Key))
                            ofs.WriteLine(
                                $"{ti.ToTitleCase(kvp.Key)},{vgp.Key},{vgp.Value}");
            }
            

            // ---------------------------------------------------------
            // extract time of day

            // by dayOfWeek, timeOfDay
            var timeOfDay = be.Records().AsParallel()
                .GroupBy(d => d.LocalTime.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.GroupBy(d => d.LocalTime.ToString("HH"))
                    .ToDictionary(d => d.Key, d => d.Average(i=>i.Compound)));

            using (var ofs = new StreamWriter($@"..\..\sentimentTimeOfDayActivity.csv"))
            {
                ofs.WriteLine("DayOfWeek,Hour,Average");
                foreach (var dvp in timeOfDay)
                    foreach (var kvp in dvp.Value)
                        ofs.WriteLine($"{dvp.Key},{kvp.Key},{kvp.Value}");
            }


           
        }


    }
}