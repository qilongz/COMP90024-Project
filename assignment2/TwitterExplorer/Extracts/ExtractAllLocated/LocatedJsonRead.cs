using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using TwitterUtil.TweetSummary;
using TwitterUtil.Twitter;

namespace ExtractAllLocated
{
    public class LocatedJsonRead
    {
        private static Encoding _encoding;
        private DataContractJsonSerializer _ser;


        public LocatedJsonRead(List<string> srcLocs, int expectedSize)
        {
            SrcLocs = srcLocs;
            ExpectedSize = expectedSize;
            Records = new List<LocatedTweet>(ExpectedSize);
        }


        private LocatedJsonRead(LocatedJsonRead src)
        {
            ExpectedSize = src.ExpectedSize;
            Records = new List<LocatedTweet>(ExpectedSize);
            _ser = new DataContractJsonSerializer(typeof(UniTwitterRow));
        }


        public List<string> SrcLocs { get; }
        public List<LocatedTweet> Records { get; }
        public int ExpectedSize { get; }
        public bool SingleThreaded { get; set; }


        protected IEnumerable<Tuple<string, string>> GetLinesFromFiles()
        {
            var cnt = 0;
            foreach (var srcLoc in SrcLocs)
            {
                var directory = new DirectoryInfo(srcLoc);

                foreach (var fi in directory.EnumerateFiles("*.json", SearchOption.AllDirectories))
                    using (var ifs = new StreamReader(
                        new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        var ln = ifs.ReadLine(); // skip first  line
                        _encoding = ifs.CurrentEncoding;
                        Console.WriteLine($"\n{fi.FullName}");

                        while ((ln = ifs.ReadLine()) != null)
                            if (!string.IsNullOrWhiteSpace(ln) && ln.Length > 10)
                            {
                                if (++cnt % 100000 == 0) Console.WriteLine($"done {cnt,10:N0} ...");
                                yield return new Tuple<string, string>(fi.Name, ln);
                            }
                    }
            }
        }

        public void DoLoad()
        {
            if (SingleThreaded)
            {
                _ser = new DataContractJsonSerializer(typeof(UniTwitterRow));
                long cnt = 1;
                foreach (var ln in GetLinesFromFiles()) Process(cnt++, ln);
            }
            else
            {
                var obj = new object();

                Parallel.ForEach(
                    GetLinesFromFiles(), // files to process
                    () => new LocatedJsonRead(this),
                    (line, state, cnt, partial) => partial.Process(cnt, line),
                    partial =>
                    {
                        lock (obj)
                        {
                            Records.AddRange(partial.Records);
                        }
                    });
            }
        }


        public LocatedJsonRead Process(long cnt, Tuple<string, string> item)
        {
            var bytes = _encoding.GetBytes(item.Item2);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (UniTwitterRow) _ser.ReadObject(sf);

                    if (!(row.Doc.Coordinates != null || row.Doc.Place != null)) return this;


                    var tm = DateTime.ParseExact(row.Doc.CreatedAt,
                        "ddd MMM dd HH:mm:ss +0000 yyyy", null, DateTimeStyles.None);


                    var post = new LocatedTweet
                    {
                        Location = row.Key[0],
                        PostId = row.Id,
                        CreateTime = tm,
                        Text = row.Doc.Text,
                        UserIdStr = row.Doc.User.IdStr,
                        UserName = row.Doc.User.Name,
                        RecordId = cnt,
                        Source = row.Doc.Source,
                        File = item.Item1
                    };

                    if (row.Doc.User.UtcOffset.HasValue)
                        post.UtcOffset = row.Doc.User.UtcOffset.Value;

                    if (row.Doc.User.TimeZone != null)
                        post.TimeZone = row.Doc.User.TimeZone;

                    if (row.Doc.Entities.Urls.Any())
                        post.ExpandedUrl = row.Doc.Entities.Urls.First().ExpandedUrl;

                    if (row.Doc.Coordinates != null)
                    {
                        post.GeoEnabled = true;

                        if (row.Doc.Coordinates.Coord[0].HasValue)
                            post.Xloc = row.Doc.Coordinates.Coord[0].Value;

                        if (row.Doc.Coordinates.Coord[1].HasValue)
                            post.Yloc = row.Doc.Coordinates.Coord[1].Value;
                    }

                    if (row.Doc.Place != null)
                    {
                        post.HasPlace = true;
                        post.PlaceId = row.Doc.Place.Id;
                        post.PlaceType = row.Doc.Place.PlaceType;
                        post.PlaceName = row.Doc.Place.Name;

                        post.BoxCoordinates = row.Doc.Place.BoundingBox.Coordinates[0];
                    }


                    if (row.Doc.Entities.Hashtags != null)
                        post.HashTags = row.Doc.Entities.Hashtags.Select(x => x.Text).ToList();


                    Records.Add(post);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Problem {ex.Message}");
                }
            }

            return this;
        }
    }
}