using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterUtil.Extraction;

namespace GenerateStats
{
    public class BatchEngine<TS, TE, TV> where TE : IExtractor<TS, TV>, new()
    {
        private const int WaitTimeOut = 4000;
        private bool _init;

        public BatchEngine(int engineCnt, IList<string> srcLocs)
        {
            Current = -1;
            EngineCnt = engineCnt;
            SrcLocs = srcLocs;

            Agents = new List<EngineAgent<LocationAgent<TS, TE, TV>>>(EngineCnt);
        }

        public int EngineCnt { get; }
        public IList<string> SrcLocs { get; }


        public List<EngineAgent<LocationAgent<TS, TE, TV>>> Agents { get; }
        public int Current { get; set; }
        public bool GetGeoLocatedOnly { get; set; }

        public int Count => Agents.Sum(x => x.Process.Records.Count);

        private void Init(Encoding encoding)
        {
            for (var i = 1; i < EngineCnt; i++)
            {
                var agt = new EngineAgent<LocationAgent<TS, TE, TV>>();
                agt.Initialise(i, encoding, GetGeoLocatedOnly);
                Agents.Add(agt);
            }

            _init = true;
        }

        public IEnumerable<TV> Records()
        {
            foreach (var eng in Agents)
            foreach (var rec in eng.Process.Records)
                yield return rec;
        }


        public void Process()
        {
            long cnt = 0;
            foreach (var srcLoc in SrcLocs)
            {
                var directory = new DirectoryInfo(srcLoc);

                foreach (var fi in directory.EnumerateFiles("*.json", SearchOption.AllDirectories))
                    using (var ifs = new StreamReader(
                        new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        Console.WriteLine($"\n{fi.FullName}");
                        string ln;

                        while ((ln = ifs.ReadLine()) != null)
                        {
                            if (!_init) Init(ifs.CurrentEncoding);

                            if (!string.IsNullOrWhiteSpace(ln) && ln.Length > 10)
                            {
                                if (++cnt % 500000 == 0)
                                    Console.WriteLine(
                                        $"done {cnt,12:N0}  {Agents.Sum(x => x.Process.Records.Count),12:N0}...");

                                AllocateToEngine(cnt, ln);
                            }
                        }
                    }
            }

            Harvest();
        }


        public void AllocateToEngine(long cnt, string line)
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            var item = Agents.FirstOrDefault(x => !x.InUse);

            while (item == null)
            {
                // if non free, wait until existing task finishes

                var active = GetActiveTasks();
                var offset = Task.WaitAny(active, WaitTimeOut);

                if (offset >= 0 && offset != WaitHandle.WaitTimeout)
                {
                    foreach (var done in Agents.Where(x => x.InUse &&
                                                           x.Task != null &&
                                                           x.Task.IsCompleted))
                        done.ReleaseTask();
                    item = Agents.First(x => !x.InUse);
                }
            }


            // launch task
            item.SetTask(cnt, Task.Run(() => item.Process.Analyse(line)));
        }


        public void Harvest()
        {
            Task[] remaining;
            while ((remaining = GetActiveTasks()) != null && remaining.Length > 0)
            {
                // wait for free
                var offset = Task.WaitAny(remaining, WaitTimeOut);

                if (offset >= 0 && offset != WaitHandle.WaitTimeout)
                    foreach (var done in Agents.Where(x => x.InUse &&
                                                           x.Task != null &&
                                                           x.Task.IsCompleted))
                        // all done
                        done.ReleaseTask();
            }
        }


        public EngineAgent<LocationAgent<TS, TE, TV>> Next()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            if (++Current >= Agents.Count) Current = 0;

            return Agents[Current];
        }

        public IEnumerable<EngineAgent<LocationAgent<TS, TE, TV>>> GetFreeList()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            var eng = Agents.First(x => !x.InUse);

            if (eng == null) yield break;
            yield return eng;
        }


        public EngineAgent<LocationAgent<TS, TE, TV>> GetFreeOne()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            return Agents.First(x => !x.InUse);
        }

        private Task[] GetActiveTasks()
        {
            return Agents.Where(x => x.InUse).Select(x => x.Task).ToArray();
        }
    }
}