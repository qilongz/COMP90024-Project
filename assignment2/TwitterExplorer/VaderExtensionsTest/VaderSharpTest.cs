using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaderSharp;

namespace VaderExtensionsTest
{
    [TestClass]
    public class VaderSharpTest
    {
        private SentimentIntensityAnalyzer _analyzer;

        [TestInitialize]
        public void Init()
        {
            _analyzer = new SentimentIntensityAnalyzer();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var txt = "VADER is smart, handsome, and funny.";

            var res = _analyzer.PolarityScores(txt);

            Assert.AreEqual(0.746, res.Positive, "not as expected");
        }

        [TestMethod]
        public void TestMethod2()
        {
            var txt =
                "Women's and Children's Hospital,North Adelaide,SA,,@SAfridiOfficial CONGRATULATIONSSSS❤❤💞💞😊😊💙💙💙💙";
            var special = "💙"[0];

            var res = _analyzer.PolarityScores(txt);

            Assert.AreEqual(1,res.Neutral, "not as expected");
        }
    }
}