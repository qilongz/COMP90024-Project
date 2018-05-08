using System.Collections.Generic;
using System.Linq;
using TwitterUtil.Geo;
using TwitterUtil.TweetSummary;
using TwitterUtil.Util;

namespace FilterHospitals.Hospital
{
    public class Target
    {
        public HospitalDescription Description { get; set; }
        public BoundingBox Box { get; set; }
    }


    public class TargetRegions
    {
        public TargetRegions(IList<HospitalDescription> src)
        {
            var tgts = src.Select(x => new Target
            {
                Description = x,
                Box = new BoundingBox(x.Latitude, x.Longitude)
            });

            Targets = new FixedSortedList<double, Target>(tgts.ToDictionary(x => x.Box.Xmin));
        }

        // ordered by Xmin
        public FixedSortedList<double, Target> Targets { get; }


        public bool Find(TagPosterDetails tag, out Target tgt)
        {
            tgt = null;
            if (!tag.Xloc.HasValue) return false;
            var longitude = tag.Xloc.Value;

            // skip if longitude too extreme
            foreach (var possible in Targets.From(longitude))
            {
                if (tag.Yloc != null && possible.Value.Box.InBox(tag.Yloc.Value, longitude))
                {
                    tgt = possible.Value;
                    return true;
                }

                if (longitude < possible.Value.Box.Xmin)
                    return false; // reach end of possibles
            }

            return false;
        }
    }
}