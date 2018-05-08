using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterUtil.Batch
{
    public class BatchEngine<TAgent, TObject>
        where TAgent : IGetAgent<TObject>, new()
        where TObject : new()
    {
        private const int WaitTimeOut = 4000;
      public  int EngineCnt { get; }


        public BatchEngine(int engineCnt ,TAgent agent)
        {
            Agents = new List<EngineAgent<TAgent, TObject>>(EngineCnt);
            Current = -1;
            EngineCnt = engineCnt;

            for (var i = 1; i < EngineCnt; i++)
            {
                var agt = new EngineAgent<TAgent, TObject>();
                agt.Initialise(agent);
                Agents.Add(agt);
            }
        }

        public List<EngineAgent<TAgent, TObject>> Agents { get; }
        public int Current { get; set; }

        public void Process(IEnumerable<string> links)
        {
            foreach (var link in links)
                AllocateToEngine(link);

            Harvest();
        }


        public void AllocateToEngine(string link)
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
            item.SetTask(link, Task.Run(() => item.Process.Get(link)));
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


        public EngineAgent<TAgent, TObject> Next()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            if (++Current >= Agents.Count) Current = 0;

            return Agents[Current];
        }

        public IEnumerable<EngineAgent<TAgent, TObject>> GetFreeList()
        {
            if (Agents.Count == 0) throw new ArgumentException("No agents defined");
            var eng = Agents.First(x => !x.InUse);

            if (eng == null) yield break;
            yield return eng;
        }


        public EngineAgent<TAgent, TObject> GetFreeOne()
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