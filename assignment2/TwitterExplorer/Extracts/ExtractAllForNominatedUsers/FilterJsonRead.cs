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
using VaderSharp;

namespace ExtractAllForNominatedUsers
{
    public class FilterJsonRead
    {
        private static Encoding _encoding;
        private static readonly object Obj = new object();
        private readonly DataContractJsonSerializer _ser;
        private readonly SentimentIntensityAnalyzer analyzer;

        public FilterJsonRead(List<string> srcLocs, int expectedSize, HashSet<string> ids)
        {
            SrcLocs = srcLocs;
            Ids = ids;
            ExpectedSize = expectedSize;
            Records = new List<TweetScore>(ExpectedSize);
        }


        private FilterJsonRead(FilterJsonRead src)
        {
            Ids = src.Ids;
            ExpectedSize = src.ExpectedSize;
            Records = new List<TweetScore>(ExpectedSize);
            _ser = new DataContractJsonSerializer(typeof(UniTwitterRow));
            analyzer = new SentimentIntensityAnalyzer();
        }


        public List<string> SrcLocs { get; }
        public HashSet<string> Ids { get; }
        public List<TweetScore> Records { get; }
        public int ExpectedSize { get; }


        protected IEnumerable<string> GetLinesFromFiles()
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
                                yield return ln;
                            }
                    }
            }
        }

        public void DoLoad()
        {
            Parallel.ForEach(
                GetLinesFromFiles(), // files to process
                () => new FilterJsonRead(this),
                (line, state, cnt, partial) => partial.Process(cnt, line),
                partial =>
                {
                    lock (Obj)
                    {
                        Records.AddRange(partial.Records);
                    }
                });
        }


        public FilterJsonRead Process(long cnt, string line)
        {
            var bytes = _encoding.GetBytes(line);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (UniTwitterRow) _ser.ReadObject(sf);

                    if (!Ids.Contains(row.Doc.User.IdStr)) return this;

                    var tm = DateTime.ParseExact(row.Doc.CreatedAt,
                        "ddd MMM dd HH:mm:ss +0000 yyyy", null, DateTimeStyles.None);

                    var res = analyzer.PolarityScores(row.Doc.Text);

                    var post = new TweetScore
                    {
                        Location = row.Key[0],
                        PostId = row.Id,
                        CreateTime = tm,
                        Text = row.Doc.Text,
                        UserIdStr = row.Doc.User.IdStr,
                        UserName = row.Doc.User.Name,
                        RecordId = cnt,
                        Source = row.Doc.Source,
                        Negative = res.Negative,
                        Neutral = res.Neutral,
                        Positive = res.Positive,
                        Compound = res.Compound
                    };

                    if (row.Doc.User.TimeZone != null)
                        post.TimeZone = row.Doc.User.TimeZone;

                    if (row.Doc.Coordinates != null)
                    {
                        post.GeoEnabled = true;

                        if (row.Doc.Coordinates.Coord[0].HasValue)
                            post.Xloc = row.Doc.Coordinates.Coord[0].Value;

                        if (row.Doc.Coordinates.Coord[1].HasValue)
                            post.Yloc = row.Doc.Coordinates.Coord[1].Value;
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