using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Json;
using System.Text;
using TwitterUtil.Extraction;

namespace GenerateStats
{
    public class LocationAgent<TS, TE, TV> : IEngineAgent where TE : IExtractor<TS, TV>, new()
    {
        private Encoding _encoding;
        private TE _extractor;
        private DataContractJsonSerializer _inSer;

        public int EngineId { get; set; }
        public List<TV> Records { get; private set; }
        public bool GeoOnly { get; set; }


        public void Initialise(int engId, Encoding encoding, bool geoOnly)
        {
            EngineId = engId;
            GeoOnly = geoOnly;
            Records = new List<TV>();

            _encoding = encoding;
            _inSer = new DataContractJsonSerializer(typeof(TS));
            _extractor = new TE();
            ;
        }


        public void Analyse(string line)
        {
            var bytes = _encoding.GetBytes(line);
            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (TS) _inSer.ReadObject(sf);

                    if (GeoOnly && !_extractor.IsGeo(row)) return;

                    var post = _extractor.Extract(row);
                    Records.Add(post);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Problem {ex.Message}");
                }
            }
        }
    }
}