using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TwitterUtil.TweetSummary
{
    public class JsonRead<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Encoding _encoding;
        private DataContractJsonSerializer _ser;

        public JsonRead(IList<string> srcFile, int expectedSize = 1000000)
        {
            SrcLocs = srcFile;
            ExpectedSize = expectedSize;
            Records = new List<T>(ExpectedSize);
        }


        private JsonRead(JsonRead<T> src)
        {
            // reduce allocation to the expected proportion needed by this thread
            var engineCnt = Environment.ProcessorCount - 1;
            ExpectedSize /= engineCnt;

            Init();
        }


        public IList<string> SrcLocs { get; }
        public List<T> Records { get; private set; }
        public bool SingleThreaded { get; set; }
        public int ExpectedSize { get; }


        private void Init()
        {
            _ser = new DataContractJsonSerializer(typeof(T));
            Records = new List<T>(ExpectedSize);
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
                        string ln;
                        while ((ln = ifs.ReadLine()) != null)
                        {
                            if (_encoding == null) _encoding = ifs.CurrentEncoding;

                            if (!string.IsNullOrWhiteSpace(ln) && ln.Length > 10)
                            {
                                if (++cnt % 100000 == 0) Console.WriteLine($"done {cnt,12:N0} ...");
                                yield return ln;
                            }
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
                    () => new JsonRead<T>(this),
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


        public JsonRead<T> Process(long cnt, string line)
        {
            var bytes = _encoding.GetBytes(line);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (T) _ser.ReadObject(sf);
                    Records.Add(row);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Problem {ex.Message}");
                }
            }

            return this;
        }


        public void WriteToFile(string fname)
        {
            const byte nl = (byte) '\n';
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var fs = File.Open(fname, FileMode.Create))
            {
                foreach (var rec in Records)
                {
                    ser.WriteObject(fs, rec);
                    fs.WriteByte(nl);
                }
            }
        }
    }
}