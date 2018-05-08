using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterUtil;
using TwitterUtil.TweetSummary;
using TwitterUtil.Twitter;

namespace TwitterExplore
{
    public class JsonRead
    {
        private const string FindTags = @"#([^# ]+)";
        private static Encoding _encoding;
        private Regex _re;
        private DataContractJsonSerializer _ser;

        public JsonRead(List<string> srcLocs, bool geoTaggedOnly = true, int expectedSize = 1000000)
        {
            SrcLocs = srcLocs;
            GeoTaggedOnly = geoTaggedOnly;
            ExpectedSize = expectedSize;
            Records = new List<TagPosterDetails>(ExpectedSize);
        }


        private JsonRead(JsonRead src)
        {
            // reduce allocation to the expected proportion
            // need by this thread
            var engineCnt = Environment.ProcessorCount - 1;
            ExpectedSize /= engineCnt;

            Init();
        }


        public List<string> SrcLocs { get; }
        public List<TagPosterDetails> Records { get; private set; }
        public bool SingleThreaded { get; set; }
        public int ExpectedSize { get; }
        public bool GeoTaggedOnly { get; set; }

        private void Init()
        {
            _ser = new DataContractJsonSerializer(typeof(UniTwitterRow));
            _re = new Regex(FindTags, RegexOptions.Compiled);
            Records = new List<TagPosterDetails>(ExpectedSize);
        }

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
            if (SingleThreaded)
            {
                Init();
                var cnt = 0;
                foreach (var fi in GetLinesFromFiles()) Process(++cnt, fi);
            }
            else
            {
                var obj = new object();

                Parallel.ForEach(
                    GetLinesFromFiles(), // files to process
                    () => new JsonRead(this),
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


        public JsonRead Process(long cnt, string line)
        {
            var bytes = _encoding.GetBytes(line);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (UniTwitterRow) _ser.ReadObject(sf);
                    if (GeoTaggedOnly && row.Doc.Coordinates == null) return this;

                    var tm = DateTime.ParseExact(row.Doc.CreatedAt, "ddd MMM dd HH:mm:ss +0000 yyyy", null,
                        DateTimeStyles.None);

                    var post = new TagPosterDetails
                    {
                        Location = row.Key[0],
                        PostId = row.Id,
                        CreateTime = tm,
                        Text = row.Doc.Text,
                        UserIdStr = row.Doc.User.IdStr,
                        UserName = row.Doc.User.Name,
                        RecordId = cnt,
                        Source = row.Doc.Source
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


        public void DumpToFile(string loc, bool geoTaggedOnly = false)
        {
            using (var ofs = new StreamWriter(loc))
            {
                ofs.WriteLine(TagPosterDetails.GetHeader());

                foreach (var rec in Records)
                    if (geoTaggedOnly)
                    {
                        if (rec.GeoEnabled)
                            ofs.WriteLine(rec.ToString());
                    }
                    else
                    {
                        ofs.WriteLine(rec.ToString());
                    }
            }
        }
    }
}