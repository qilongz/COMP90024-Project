using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using TwitterUtil.TweetSummary;
using TwitterUtil.Twitter;

namespace ExtractAllLocated
{
    public class LocatedJsonReadAgent : IDisposable
    {
        private const byte Nl = (byte) '\n';

        private Encoding _encoding;
        private DataContractJsonSerializer _inSer;

        private FileStream _ofs;
        private DataContractJsonSerializer _outSer;

        public string TgtLocation { get; private set; }
        public bool GeoOnly { get; private set; }

        public void Dispose()
        {
            if (_ofs != null)
            {
                _ofs.Flush();
                _ofs.Close();
            }
        }


        public void Initialise(int engId, string tgtLocation, Encoding encoding, bool geoOnly)
        {
            TgtLocation = tgtLocation;
            GeoOnly = geoOnly;
            _encoding = encoding;
            _inSer = new DataContractJsonSerializer(typeof(UniTwitterRow));


            _outSer = new DataContractJsonSerializer(typeof(LocatedTweet));
            _ofs = File.Open(Path.Combine(TgtLocation, $"twitter-geolocated-{engId}.json"), FileMode.Create);
        }


        public void Extract(long cnt, string fileName, string line)
        {
            var bytes = _encoding.GetBytes(line);

            using (var sf = new MemoryStream(bytes))
            {
                try
                {
                    var row = (UniTwitterRow) _inSer.ReadObject(sf);

                    if (GeoOnly && !(row.Doc.Coordinates != null || row.Doc.Place != null)) return;


                    var tm = DateTime.ParseExact(row.Doc.CreatedAt,
                        "ddd MMM dd HH:mm:ss +0000 yyyy", null, DateTimeStyles.None);


                    var post = new LocatedTweet
                    {
                        Location = row.Key[0],
                        PostId = row.Id,
                        CreateTime = tm,
                        Text = row.Doc.Text,
                        UserIdStr = row.Doc.User.IdStr,
                        UserName = row.Doc.User.Name,
                        RecordId = cnt,
                        Source = row.Doc.Source,
                        File = fileName
                    };

                    if (row.Doc.User.UtcOffset.HasValue)
                        post.UtcOffset = row.Doc.User.UtcOffset.Value;

                    if (row.Doc.User.TimeZone != null)
                        post.TimeZone = row.Doc.User.TimeZone;

                    if (row.Doc.Entities.Urls.Any())
                        post.ExpandedUrl = row.Doc.Entities.Urls.First().ExpandedUrl;

                    if (row.Doc.Coordinates != null)
                    {
                        post.GeoEnabled = true;

                        if (row.Doc.Coordinates.Coord[0].HasValue)
                            post.Xloc = row.Doc.Coordinates.Coord[0].Value;

                        if (row.Doc.Coordinates.Coord[1].HasValue)
                            post.Yloc = row.Doc.Coordinates.Coord[1].Value;
                    }

                    if (row.Doc.Place != null)
                    {
                        post.HasPlace = true;
                        post.PlaceId = row.Doc.Place.Id;
                        post.PlaceType = row.Doc.Place.PlaceType;
                        post.PlaceName = row.Doc.Place.Name;

                        post.BoxCoordinates = row.Doc.Place.BoundingBox.Coordinates[0];
                    }


                    if (row.Doc.Entities.Hashtags != null)
                        post.HashTags = row.Doc.Entities.Hashtags.Select(x => x.Text).ToList();


                    _outSer.WriteObject(_ofs, post);
                    _ofs.WriteByte(Nl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Problem {ex.Message}");
                }
            }
        }
    }
}