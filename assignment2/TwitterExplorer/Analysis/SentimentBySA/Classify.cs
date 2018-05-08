using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;
using VaderExtended;

namespace SentimentBySA
{
    public class Classify
    {
        private readonly SentimentIntensityAnalyzer _analyzer;
        private readonly object _obj = new object();

        public Classify(List<TagPosterDetails> items, SADictionary sad)
        {
            Records = items;
            Sad = sad;
            _analyzer = new SentimentIntensityAnalyzer();
        }


        private Classify(Classify src)
        {
            _analyzer = new SentimentIntensityAnalyzer(src._analyzer);
            Sad = new SADictionary(src.Sad.SASets);
        }


        public List<LocatedScore> Scores { get; } = new List<LocatedScore>();
        public SADictionary Sad { get; }

        public List<TagPosterDetails> Records { get; }
        public bool SingleThreaded { get; set; }


        public void DoClassification()
        {
            if (SingleThreaded)
            {
                var cnt = 0;
                foreach (var post in Records)
                    Process(++cnt, post);
            }
            else
            {
                Parallel.ForEach(
                    GetNextPost(),
                    () => new Classify(this),
                    (line, state, cnt, partial) => partial.Process(cnt, line),
                    partial =>
                    {
                        lock (_obj)
                        {
                            Scores.AddRange(partial.Scores);
                            Sad.Merge(partial.Sad);
                        }
                    });
            }
        }


        public IEnumerable<TagPosterDetails> GetNextPost()
        {
            for (var i = 1; i < Records.Count; i++)
            {
                if (i % 10000 == 0) Console.WriteLine($"analysed {i,12:N0} ...");
                yield return Records[i];
            }
        }


        public Classify Process(long cnt, TagPosterDetails post)
        {
            // find areas
            if (!post.Xloc.HasValue || !post.Yloc.HasValue) return this;

            var pt = new LatLong(post.Xloc.Value, post.Yloc.Value);

            var regions = Sad.WhatRegions(pt);
            var res = _analyzer.PolarityScores(post.Text);
            var stamp = post.LocalTime;

            var score = new LocatedScore(res.Compound, stamp, regions);
            Scores.Add(score);

            return this;
        }
    }
}