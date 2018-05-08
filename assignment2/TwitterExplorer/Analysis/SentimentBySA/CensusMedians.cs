using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using TwitterUtil.Geo;

namespace SentimentBySA
{
    [DataContract]
    public class CensusMedians
    {
        [DataMember] public Features Features { get; set; } = new Features();


        public void TransformFeatures()
        {
            Features.ForEach(x => x.Transform());
        }


      


        public static void WriteSample()
        {
            // test out
            var ocm = new CensusMedians();
            ocm.Features.Add(new Feature
            {
                BoundaryDescription = "147.7108,-37.5051 150.3791,-33.8878",
                Parameters = new Aurin {Name = "test"},
                LocationDescription = new PolygonDescription
                {
                    "149.9163,-37.074 149.9162,-37.074 149.9162,-37.0739 149.9162,-37.0739 149.9163,-37.0739 149.9163,-37.0739 149.9163,-37.074",
                    "149.9163,-37.074 149.9162,-37.074 149.9162,-37.0739 149.9162,-37.0739 149.9163,-37.0739 149.9163,-37.0739 149.9163,-37.074"
                }
            });


            var ser = new DataContractSerializer(typeof(CensusMedians));
            using (var fs = File.Open(@"..\..\test.xml", FileMode.Create))
            using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(
                XmlWriter.Create(fs, new XmlWriterSettings
                {
                    Indent = true,
                    CloseOutput = false,
                    ConformanceLevel = ConformanceLevel.Fragment
                })))
            {
                ser.WriteObject(writer, ocm);
                writer.Flush();
                writer.Close();
            }
        }
    }
}