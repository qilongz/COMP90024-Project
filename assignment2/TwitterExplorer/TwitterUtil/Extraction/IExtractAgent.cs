using System.Text;

namespace TwitterUtil.Extraction
{
    public interface IExtractAgent
    {
        void Initialise(int cnt, Encoding encoding);
    }
}