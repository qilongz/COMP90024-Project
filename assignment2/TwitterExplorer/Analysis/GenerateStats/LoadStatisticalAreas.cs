using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using TwitterUtil.Geo;

namespace GenerateStats
{
    internal class LoadStatisticalAreas
    {
        private readonly XslCompiledTransform _xslt;

        public LoadStatisticalAreas()
        {
            var assembly = typeof(LoadStatisticalAreas).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("GenerateStats.extract.xslt"))
            using (var xsltStream = new StreamReader(stream))
            {
                _xslt = new XslCompiledTransform();
                _xslt.Load(XmlReader.Create(xsltStream));
            }
        }


        public Features GetFeatures(string srcFile)
        {
            var data = new XPathDocument(srcFile);
            var extract = new XDocument();
            using (var dwriter = extract.CreateWriter())
            {
                _xslt.Transform(data, dwriter);
            }


            var ser = new DataContractSerializer(typeof(CensusMedians));
            using (var xreader = extract.CreateReader())
            {
                var cm = (CensusMedians) ser.ReadObject(xreader, false);
                cm.TransformFeatures();

                return cm.Features;
            }
        }
    }
}