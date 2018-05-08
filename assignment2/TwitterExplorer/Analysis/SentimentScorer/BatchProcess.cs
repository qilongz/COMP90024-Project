using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SentimentScorer
{
    public class BatchEngine
    {
        private const int WaitTimeOut = 4000;
        private bool _init;

        public BatchEngine(int engineCnt, List<string> srcLocs, string tgtLoc)
        {
            Current = -1;
            EngineCnt = engineCnt;
            SrcLocs = srcLocs;
            TgtLoc = tgtLoc;
            Agents = new List<EngineAgent>(EngineCnt);
        }

        public int EngineCnt { get; }
        public List<string> SrcLocs { get; }
        public string TgtLoc { get; }

        public List<EngineAgent> Agents { get; }
        public int Current { get; set; }
        public bool GetGeoLocatedOnly { get; set; }

        private void Init(Encoding encoding)
        {
            for (var i = 1; i < EngineCnt; i++)
            {
                var agt = new EngineAgent();
                agt.Initialise(i, TgtLoc, encoding);
                Agents.Add(agt);
            }

            _init = true;
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
                                if (++cnt % 100000 == 0) Console.WriteLine($"done {cnt,10:N0} ...");

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


        public EngineAgent Next()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            if (++Current >= Agents.Count) Current = 0;

            return Agents[Current];
        }

        public IEnumerable<EngineAgent> GetFreeList()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            var eng = Agents.First(x => !x.InUse);

            if (eng == null) yield break;
            yield return eng;
        }


        public EngineAgent GetFreeOne()
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