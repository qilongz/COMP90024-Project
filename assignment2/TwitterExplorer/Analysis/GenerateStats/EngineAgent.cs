using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GenerateStats
{
    [DebuggerDisplay("InUse: {InUse} Id: {Number}")]
    public class EngineAgent<T> where T : IEngineAgent, new()
    {
        public EngineAgent()
        {
            Process = new T();
        }

        public long Number { get; private set; }
        public bool InUse { get; private set; }

        public T Process { get; }
        public Task Task { get; set; }

        public void Initialise(int engId, Encoding encoding, bool geoOnly) =>
            Process.Initialise(engId, encoding, geoOnly);

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


    public interface IEngineAgent
    {
        void Initialise(int engId, Encoding encoding, bool  geoOnly);
    }
}