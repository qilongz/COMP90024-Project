using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using TwitterUtil.TweetSummary;
using VaderExtended;

namespace SentimentScorer
{
    public class ScorerJsonReadAgent : IDisposable
    {
        private const byte Nl = (byte) '\n';

        private SentimentIntensityAnalyzer _analyzer;

        private Encoding _encoding;
        private DataContractJsonSerializer _inSer;

        private FileStream _ofs;
        private DataContractJsonSerializer _outSer;

        public string TgtLocation { get; private set; }
    

        public void Dispose()
        {
            if (_ofs != null)
            {
                _ofs.Flush();
                _ofs.Close();
            }
        }


        public void Initialise(int engId, string tgtLocation, Encoding encoding)
        {
            TgtLocation = tgtLocation;
          
            _encoding = encoding;
            _inSer = new DataContractJsonSerializer(typeof(TweetScore));
            _analyzer = new SentimentIntensityAnalyzer();

            _outSer = new DataContractJsonSerializer(typeof(TweetScore));
            _ofs = File.Open(Path.Combine(TgtLocation, $"twitter-TweetScore-{engId}.json"), FileMode.Create);
        }


        public void Analyse(string line)
        {
            var bytes = _encoding.GetBytes(line);
            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var post = (TweetScore) _inSer.ReadObject(sf);

                    var res = _analyzer.PolarityScores(post.Text);

                    post.Compound = res.Compound;
                    post.Negative = res.Negative;
                    post.Neutral = res.Neutral;
                    post.Positive = res.Positive;

                    _outSer.WriteObject(_ofs, post);
                    _ofs.WriteByte(Nl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Problem {ex.Message}");
                }
            }
        }
    }
}