using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ExtractAllLocated
{
    [DebuggerDisplay("InUse: {InUse} Id: {Number}")]
    public class EngineAgent
    {
        public EngineAgent()
        {
            Process = new LocatedJsonReadAgent();
        }

        public long Number { get; private set; }
        public bool InUse { get; private set; }

        public LocatedJsonReadAgent Process { get; }

        public Task Task { get; set; }

        public void Initialise(int engId, string tgtLocation, Encoding encoding, bool geoOnly) =>
            Process.Initialise(engId, tgtLocation, encoding,geoOnly);

        public void ReleaseTask()
        {
            Task = null;
            Number = 0;
            InUse = false;
        }

        public void SetTask(long cnt, Task task)
        {
            Task = task;
            Number = cnt;
            InUse = true;
        }
    }
}