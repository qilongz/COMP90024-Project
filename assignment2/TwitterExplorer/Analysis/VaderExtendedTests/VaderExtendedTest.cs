using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaderExtended;

namespace VaderExtendedTests
{
    [TestClass]
    public class VaderExtendedTest
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
                "before ❤💞😊💙 Women's and Children's Hospital,North Adelaide,SA,,@SAfridiOfficial CONGRATULATIONSSSS❤❤💞💞😊😊💙💙💙💙 after";
            var special = "💙"[0];

            var res = _analyzer.PolarityScores(txt);

            Assert.IsTrue(res.Positive > 0, "not as expected");
        }


        [TestMethod]
        public void TestMethodBlueHeart()
        {
            var txt = "💙";
            var res = _analyzer.PolarityScores(txt);

            Assert.IsTrue(res.Negative > 0, "not as expected");
        }


        [TestMethod]
        public void TestMethodRedHeart()
        {
            var txt = "❤";
            var res = _analyzer.PolarityScores(txt);

            Assert.IsTrue(res.Positive > 0, "not as expected");
        }

        [TestMethod]
        public void TestMethodOldStyle()
        {
            var txt = "0:)";
            var res = _analyzer.PolarityScores(txt);

            Assert.IsTrue(res.Positive > 0, "not as expected");
        }
    }
}