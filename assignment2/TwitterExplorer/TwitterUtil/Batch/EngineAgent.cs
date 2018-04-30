using System.Diagnostics;
using System.Threading.Tasks;

namespace TwitterUtil.Batch
{
    [DebuggerDisplay("InUse: {InUse} Id: {Link}")]
    public class EngineAgent<TAgent, TObject>
        where TAgent : IGetAgent<TObject>, new()
        where TObject : new()
    {
        public EngineAgent()
        {
            Process = new TAgent();
            Link = null;
        }

        public string Link { get; private set; }
        public bool InUse { get; private set; }

        public TAgent Process { get; }

        public Task Task { get; set; }

        public void Initialise(TAgent src) => Process.Initialise(src);

        public void ReleaseTask()
        {
            Task = null;
            Link = null;
            InUse = false;
        }

        public void SetTask(string link, Task task)
        {
            Task = task;
            Link = link;
            InUse = true;
        }
    }
}