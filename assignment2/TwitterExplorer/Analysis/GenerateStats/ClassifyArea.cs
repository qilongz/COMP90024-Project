using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;
using TwitterUtil.Twitter;

namespace GenerateStats
{
    public class ClassifyArea
    {
       
        private readonly object _obj = new object();

        public ClassifyArea(List<AreaSentiExtract> items, SADictionary sad)
        {
            Records = items;
            Sad = sad;
           
        }


        private ClassifyArea(ClassifyArea src)
        {
           
            Sad = new SADictionary(src.Sad.SASets);
        }


        public List<LocatedAreaScore> Scores { get; } = new List<LocatedAreaScore>();
        public SADictionary Sad { get; }

        public List<AreaSentiExtract> Records { get; }
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
                    () => new ClassifyArea(this),
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


        public IEnumerable<AreaSentiExtract> GetNextPost()
        {
            for (var i = 1; i < Records.Count; i++)
            {
                if (i % 10000 == 0) Console.WriteLine($"analysed {i,12:N0} ...");
                yield return Records[i];
            }
        }


        public ClassifyArea Process(long cnt, AreaSentiExtract post)
        {
            // find areas
          var pt = new LatLong(post.Xloc, post.Yloc);
            var regions = Sad.WhatRegions(pt);

            var score = new LocatedAreaScore(post, regions);       
                        Scores.Add(score);

            return this;
        }
    }
}