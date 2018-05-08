using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;

namespace GenerateStats
{
    public class Classify
    {
       
        private readonly object _obj = new object();

        public Classify(List<GeoSentimentParameters> items, SADictionary sad)
        {
            Records = items;
            Sad = sad;
           
        }


        private Classify(Classify src)
        {
           
            Sad = new SADictionary(src.Sad.SASets);
        }


        public List<LocatedScore> Scores { get; } = new List<LocatedScore>();
        public SADictionary Sad { get; }

        public List<GeoSentimentParameters> Records { get; }
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


        public IEnumerable<GeoSentimentParameters> GetNextPost()
        {
            for (var i = 1; i < Records.Count; i++)
            {
                if (i % 10000 == 0) Console.WriteLine($"analysed {i,12:N0} ...");
                yield return Records[i];
            }
        }


        public Classify Process(long cnt, GeoSentimentParameters post)
        {
            // find areas
          var pt = new LatLong(post.Xloc, post.Yloc);
            var regions = Sad.WhatRegions(pt);

            var score = new LocatedScore(post, regions);       
                        Scores.Add(score);

            return this;
        }
    }
}