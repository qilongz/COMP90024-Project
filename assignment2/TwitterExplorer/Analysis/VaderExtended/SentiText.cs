using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VaderExtended
{
    internal class SentiText
    {
        public SentiText(SentimentIntensityAnalyzer az, string text)
        {
            Az = az;
            Text = text;


            // convert emoji to word equivalents

            // are there any ?
            var match = Az.SubsubstituteEmoji.Match(text);
            if (match.Success)
            {
                var itr = StringInfo.GetTextElementEnumerator(text);
                var sb = new StringBuilder();

                while (itr.MoveNext())
                {
                    var el = itr.GetTextElement();

                    // just skip if can't find
                    if (az.Emojicon.TryGetValue(el, out var desc))
                    {
                        sb.Append(' ');
                        sb.Append(desc);
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(el);
                    }
                }

                ExpandedText = sb.ToString();
            }
            else
            {
                ExpandedText = text;
            }

            StripWrapingPunctuation();
        }

        public SentimentIntensityAnalyzer Az { get; }
        public string Text { get; }
        public string ExpandedText { get; }
        public List<string> WordsAndEmoticons { get; private set; }
        public List<string> WordsAndEmoticonsLowerCase { get; private set; }
        public bool IsCapDifferential { get; private set; }


        public void StripWrapingPunctuation()
        {
            var matches = Az.StripPunc.Matches(ExpandedText);
            WordsAndEmoticons = new List<string>(matches.Count);
            WordsAndEmoticonsLowerCase = new List<string>(matches.Count);

            foreach (Match match in matches)
            {
                var word = match.Groups[2].Value;
                IsCapDifferential |= !word.Any(char.IsLower);

                WordsAndEmoticons.Add(word);
                WordsAndEmoticonsLowerCase.Add(word.ToLower());
            }
        }
    }
}