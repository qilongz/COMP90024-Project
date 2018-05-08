using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterUtil.TweetSummary;
using TwitterUtil.Twitter;

namespace FilteredExtract
{
    public class FilterJsonRead
    {
        private const string Filter = "\"id_str\":\"([^\"]+)\",\"name\"";

        private static Encoding _encoding;
        private static readonly object Obj = new object();

        private readonly Regex _re;
        private readonly DataContractJsonSerializer _ser;


        public FilterJsonRead(List<string> srcLocs, int expectedSize, HashSet<string> ids)
        {
            SrcLocs = srcLocs;
            Ids = ids;
        }


        private FilterJsonRead(FilterJsonRead src)
        {
            Ids = src.Ids;
            Records = new List<TagPosterDetails>(10000);

            _ser = new DataContractJsonSerializer(typeof(UniTwitterRow));
            _re = new Regex(Filter, RegexOptions.Compiled);
        }


        public List<string> SrcLocs { get; }
        public HashSet<string> Ids { get; }
        public List<TagPosterDetails> Records { get; }


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


        public void ExtractAndSave(string tgtFile)
        {
            var fs = File.Open(tgtFile, FileMode.Create, FileAccess.Write);
            var oser = new DataContractJsonSerializer(typeof(TagPosterDetails));
            const byte nl = (byte) '\n';

            Parallel.ForEach(
                GetLinesFromFiles(), // files to process
                () => new FilterJsonRead(this),
                (line, state, cnt, partial) => partial.Process(cnt, line),
                partial =>
                {
                    lock (Obj)
                    {
                        foreach (var post in partial.Records)
                        {
                            oser.WriteObject(fs, post);
                            fs.WriteByte(nl);
                        }
                    }
                });

            fs.Close();
        }


        public FilterJsonRead Process(long cnt, Tuple<string, string> item)
        {
            // check if it's from a user of interest
            var match = _re.Match(item.Item2);

            // skip if not required
            if (!match.Success) return this;
            var userId = match.Groups[1].Captures[0].Value;
            if (!Ids.Contains(userId)) return this;


            var bytes = _encoding.GetBytes(item.Item2);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (UniTwitterRow) _ser.ReadObject(sf);

                    var tm = DateTime.ParseExact(row.Doc.CreatedAt,
                        "ddd MMM dd HH:mm:ss +0000 yyyy", null, DateTimeStyles.None);

                    var post = new TagPosterDetails
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

                    if (row.Doc.Entities.Urls.Any())
                        post.ExpandedUrl = row.Doc.Entities.Urls.First().ExpandedUrl;

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