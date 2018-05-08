using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace VaderExtended.ConfigStore
{
    /// <summary>
    ///     Proof of concept for loading the words to be used as boosters, negations etc.
    ///     Currently not used.
    /// </summary>
    public class ConfigStore
    {
        private static ConfigStore _config;

        private ConfigStore(string languageCode)
        {
            LoadConfig(languageCode);
        }

        public Dictionary<string, double> BoosterDict { get; private set; }

        public string[] Negations { get; private set; }

        public Dictionary<string, double> SpecialCaseIdioms { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="languageCode">Language code in writing style "language-country". Default is British English.</param>
        /// <returns>ConfigStore object.</returns>
        public static ConfigStore CreateConfig(string languageCode = "en-gb")
        {
            _config = _config ?? new ConfigStore(languageCode);
            return _config;
        }

        /// <summary>
        ///     Initializes the ConfigStore and loads the config file.
        /// </summary>
        /// <param name="languageCode">Language code in writing style "language-country".</param>
        private void LoadConfig(string languageCode)
        {
            var path = $@"E:\uni\Cluster and Cloud Computing\assign2\TwitterExplore\vadersharp\VaderSharp\VaderSharp\ConfigStore\strings\{languageCode}.xml";
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"{Directory.GetCurrentDirectory()} Language file was not found. Please check language code. {path}");
            var xDocument = XDocument.Load(path).Document;
            if (xDocument != null)
            {
                var root = xDocument.Root;
                LoadNegations(root);
                LoadIdioms(root);
                LoadBooster(root);
            }
        }

        /// <summary>
        ///     Loads negations from config file.
        /// </summary>
        /// <param name="root">Root element of XML document</param>
        private void LoadNegations(XElement root)
        {
            Negations = root.Descendants(XName.Get("negation")).Select(x=>x.Value).ToArray();
        }

        /// <summary>
        ///     Loads idioms from config file.
        /// </summary>
        /// <param name="root">Root element of XML document</param>
        private void LoadIdioms(XElement root)
        {
            SpecialCaseIdioms = new Dictionary<string, double>();
            var nodes = root.Descendants(XName.Get("idiom"));
            foreach (var n in nodes)
            {
                var value = double.Parse(n.Attribute(XName.Get("value"))?.Value);
                SpecialCaseIdioms.Add(n.Value, value);
            }
        }

        /// <summary>
        ///     Loads booster words from config file.
        /// </summary>
        /// <param name="root">Root element of XML document</param>
        private void LoadBooster(XElement root)
        {
            BoosterDict = new Dictionary<string, double>();
            var nodes = root.Descendants(XName.Get("booster"));
            foreach (var n in nodes)
            {
                var sign = n.Attribute(XName.Get("sign"))?.Value == "BIncr" ? 0.293 : -0.293;
                BoosterDict.Add(n.Value, sign);
            }
        }
    }
}