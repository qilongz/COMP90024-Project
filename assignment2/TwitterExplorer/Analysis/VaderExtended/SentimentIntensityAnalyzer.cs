using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace VaderExtended
{
    /// <summary>
    ///     An abstraction to represent the sentiment intensity analyzer.
    /// </summary>
    public class SentimentIntensityAnalyzer
    {
        // @"\b[^ '""]+\b"

        private const double ExclIncr = 0.292;
        private const double QuesIncrSmall = 0.18;
        private const double QuesIncrLarge = 0.96;

        public Regex StripPunc = new Regex(@"([ \.,\?\!]*)([^ ,\.]+)([ \.,\?\!]*)",
            RegexOptions.Compiled | RegexOptions.Multiline);

        public Regex SubsubstituteEmoji = new Regex(@"\p{So}|\p{Cs}", RegexOptions.Compiled | RegexOptions.Multiline);

        public SentimentIntensityAnalyzer()
        {
            var assembly = typeof(SentimentIntensityAnalyzer).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream("VaderExtended.vader_lexicon.txt"))
            using (var reader = new StreamReader(stream))
            {
                Lexicon = new Dictionary<string, double>();
                string ln;
                while ((ln = reader.ReadLine()) != null)
                {
                    var arr = ln.Trim().Split('\t');
                    Lexicon[arr[0]] = double.Parse(arr[1]);
                }
            }

            using (var stream = assembly.GetManifestResourceStream("VaderExtended.emoji_utf8_lexicon.txt"))
            using (var reader = new StreamReader(stream))
            {
                Emojicon = new Dictionary<string, string>();
                string ln;
                while ((ln = reader.ReadLine()) != null)
                {
                    var arr = ln.Trim().Split('\t');
                    Emojicon.Add(arr[0], arr[1]);
                }
            }
        }

        public SentimentIntensityAnalyzer(SentimentIntensityAnalyzer src)
        {
            Lexicon = src.Lexicon;
            Emojicon = src.Emojicon;
        }

        public Dictionary<string, double> Lexicon { get; }
        public Dictionary<string, string> Emojicon { get; }


        //     Return metrics for positive, negative and neutral sentiment based on the input text.

        public SentimentAnalysisResults PolarityScores(string input)
        {
            var sentiText = new SentiText(this, input);

            var sentiments = AnalyseBoosters(sentiText);
            sentiments = ButCheck(sentiText.WordsAndEmoticonsLowerCase, sentiments);

            return ScoreValence(sentiments, sentiText);
        }


        private List<double> AnalyseBoosters(SentiText sentiText)
        {
            var size = sentiText.WordsAndEmoticonsLowerCase.Count;
            var sentiments = new List<double>();

            for (var i = 0; i < size; i++)
            {
                var lowerCaseWord = sentiText.WordsAndEmoticonsLowerCase[i];

                double valence = 0;
                if (i < size - 1 && lowerCaseWord == "kind" && sentiText.WordsAndEmoticonsLowerCase[i + 1] == "of"
                    || SentimentUtils.BoosterDict.ContainsKey(lowerCaseWord))
                {
                    sentiments.Add(valence);
                    continue;
                }

                sentiments = SentimentValence(valence, sentiText,
                    sentiText.WordsAndEmoticons[i], lowerCaseWord,
                    i, sentiments);
            }

            return sentiments;
        }


        private List<double> SentimentValence(double valence,
            SentiText sentiText, string word,
            string lowerCaseWord, int i,
            List<double> sentiments)
        {
            if (!Lexicon.ContainsKey(lowerCaseWord))
            {
                sentiments.Add(valence);
                return sentiments;
            }

            var isCapDiff = sentiText.IsCapDifferential;


            // adjust if shouting
            valence = Lexicon[lowerCaseWord];
            if (isCapDiff && word.IsUpper())
                if (valence > 0)
                    valence += SentimentUtils.CIncr;
                else
                    valence -= SentimentUtils.CIncr;


            for (var startI = 0; startI < 3 && i > startI; startI++)
            {
                var offset = i - (startI + 1);
                var lcWord = sentiText.WordsAndEmoticonsLowerCase[offset];

                if (Lexicon.ContainsKey(lcWord)) continue;

                var s = SentimentUtils.ScalarIncDec(lcWord,
                    sentiText.WordsAndEmoticons[offset], valence, isCapDiff);

                if (startI == 1 && !s.Equals(0))
                    s = s * 0.95;
                if (startI == 2 && !s.Equals(0))
                    s = s * 0.9;
                valence = valence + s;

                valence = NeverCheck(valence, sentiText.WordsAndEmoticonsLowerCase, startI, i);

                if (startI == 2)
                    valence = IdiomsCheck(valence, sentiText.WordsAndEmoticonsLowerCase, i);
            }

            valence = LeastCheck(valence, sentiText.WordsAndEmoticonsLowerCase, i);
            sentiments.Add(valence);

            return sentiments;
        }


        private List<double> ButCheck(List<string> lowerCaseWordsAndEmoticons, List<double> sentiments)
        {
            if (!lowerCaseWordsAndEmoticons.Contains("but"))
                return sentiments;

            var butIndex = lowerCaseWordsAndEmoticons.IndexOf("but");

            for (var i = 0; i < sentiments.Count; i++)
            {
                var sentiment = sentiments[i];
                if (i < butIndex)
                {
                    sentiments.RemoveAt(i);
                    sentiments.Insert(i, sentiment * 0.5);
                }
                else if (i > butIndex)
                {
                    sentiments.RemoveAt(i);
                    sentiments.Insert(i, sentiment * 1.5);
                }
            }

            return sentiments;
        }


        private double LeastCheck(double valence, List<string> lowerCaseWordsAndEmoticons, int i)
        {
            if (i > 1 &&
                !Lexicon.ContainsKey(lowerCaseWordsAndEmoticons[i - 1]) &&
                lowerCaseWordsAndEmoticons[i - 1] == "least")
            {
                if (lowerCaseWordsAndEmoticons[i - 2] != "at" &&
                    lowerCaseWordsAndEmoticons[i - 2] != "very")
                    valence = valence * SentimentUtils.NScalar;
            }
            else if (i > 0 &&
                     !Lexicon.ContainsKey(lowerCaseWordsAndEmoticons[i - 1])
                     && lowerCaseWordsAndEmoticons[i - 1] == "least")
            {
                valence = valence * SentimentUtils.NScalar;
            }

            return valence;
        }


        private double NeverCheck(double valence, List<string> lowerCaseWordsAndEmoticons, int startI, int i)
        {
            if (startI == 0)
                if (SentimentUtils.Negated(new List<string> {lowerCaseWordsAndEmoticons[i - 1]}))
                    valence = valence * SentimentUtils.NScalar;

            if (startI == 1)
                if (lowerCaseWordsAndEmoticons[i - 2] == "never" &&
                    (lowerCaseWordsAndEmoticons[i - 1] == "so" ||
                     lowerCaseWordsAndEmoticons[i - 1] == "this"))
                    valence = valence * 1.5;
                else if (SentimentUtils.Negated(new List<string> {lowerCaseWordsAndEmoticons[i - (startI + 1)]}))
                    valence = valence * SentimentUtils.NScalar;

            if (startI == 2)
                if (lowerCaseWordsAndEmoticons[i - 3] == "never"
                    && (lowerCaseWordsAndEmoticons[i - 2] == "so" ||
                        lowerCaseWordsAndEmoticons[i - 2] == "this") ||
                    lowerCaseWordsAndEmoticons[i - 1] == "so" ||
                    lowerCaseWordsAndEmoticons[i - 1] == "this")

                    valence = valence * 1.25;
                else if (SentimentUtils.Negated(new List<string> {lowerCaseWordsAndEmoticons[i - (startI + 1)]}))
                    valence = valence * SentimentUtils.NScalar;

            return valence;
        }

        private double IdiomsCheck(double valence, List<string> lcWords, int i)
        {
            var oneZero = $"{lcWords[i - 1]} {lcWords[i]}";
            var twoOneZero = $"{lcWords[i - 2]} {lcWords[i - 1]} {lcWords[i]}";
            var twoOne = $"{lcWords[i - 2]} {lcWords[i - 1]}";
            var threeTwoOne = $"{lcWords[i - 3]} {lcWords[i - 2]} {lcWords[i - 1]}";
            var threeTwo = $"{lcWords[i - 3]} {lcWords[i - 2]}";

            string[] sequences = {oneZero, twoOneZero, twoOne, threeTwoOne, threeTwo};

            foreach (var seq in sequences)
                if (SentimentUtils.SpecialCaseIdioms.ContainsKey(seq))
                {
                    valence = SentimentUtils.SpecialCaseIdioms[seq];
                    break;
                }

            if (lcWords.Count - 1 > i)
            {
                var zeroOne = $"{lcWords[i]} {lcWords[i + 1]}";

                if (SentimentUtils.SpecialCaseIdioms.ContainsKey(zeroOne))
                    valence = SentimentUtils.SpecialCaseIdioms[zeroOne];
            }

            if (lcWords.Count - 1 > i + 1)
            {
                var zeroOneTwo = $"{lcWords[i]} {lcWords[i + 1]} {lcWords[i + 2]}";

                if (SentimentUtils.SpecialCaseIdioms.ContainsKey(zeroOneTwo))
                    valence = SentimentUtils.SpecialCaseIdioms[zeroOneTwo];
            }

            if (SentimentUtils.BoosterDict.ContainsKey(threeTwo) ||
                SentimentUtils.BoosterDict.ContainsKey(twoOne))
                valence += SentimentUtils.BDecr;

            return valence;
        }

        private double PunctuationEmphasis(string text)
        {
            return AmplifyExclamation(text) + AmplifyQuestion(text);
        }

        private double AmplifyExclamation(string text)
        {
            var epCount = text.Count(x => x == '!');

            if (epCount > 4) epCount = 4;

            return epCount * ExclIncr;
        }


        private static double AmplifyQuestion(string text)
        {
            var qmCount = text.Count(x => x == '?');

            if (qmCount < 1) return 0;
            if (qmCount <= 3) return qmCount * QuesIncrSmall;

            return QuesIncrLarge;
        }

        private static SiftSentiments SiftSentimentScores(List<double> sentiments)
        {
            var siftSentiments = new SiftSentiments();

            foreach (var sentiment in sentiments)
            {
                if (sentiment > 0)
                    siftSentiments.PosSum += sentiment + 1; //1 compensates for neutrals

                if (sentiment < 0)
                    siftSentiments.NegSum += sentiment - 1;

                if (sentiment.Equals(0))
                    siftSentiments.NeuCount++;
            }

            return siftSentiments;
        }


        private SentimentAnalysisResults ScoreValence(List<double> sentiments, SentiText text)
        {
            if (sentiments.Count == 0)
                return new SentimentAnalysisResults(); //will return with all 0

            var sum = sentiments.Sum();
            var puncAmplifier = PunctuationEmphasis(text.Text);

            sum += Math.Sign(sum) * puncAmplifier;

            var compound = SentimentUtils.Normalize(sum);
            var sifted = SiftSentimentScores(sentiments);

            if (sifted.PosSum > Math.Abs(sifted.NegSum))
                sifted.PosSum += puncAmplifier;
            else if (sifted.PosSum < Math.Abs(sifted.NegSum))
                sifted.NegSum -= puncAmplifier;

            var total = sifted.PosSum + Math.Abs(sifted.NegSum) + sifted.NeuCount;

            return new SentimentAnalysisResults
            {
                Compound = Math.Round(compound, 4),
                Positive = Math.Round(Math.Abs(sifted.PosSum / total), 3),
                Negative = Math.Round(Math.Abs(sifted.NegSum / total), 3),
                Neutral = Math.Round(Math.Abs(sifted.NeuCount / total), 3)
            };
        }

        private class SiftSentiments
        {
            public double PosSum { get; set; }
            public double NegSum { get; set; }
            public int NeuCount { get; set; }
        }
    }
}