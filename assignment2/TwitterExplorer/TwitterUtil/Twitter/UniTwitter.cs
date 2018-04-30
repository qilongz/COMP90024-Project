using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwitterUtil.Twitter
{
    [DataContract]
    public class Image
    {
        [DataMember(Name = "w")] public int W { get; set; }
        [DataMember(Name = "h")] public int H { get; set; }
        [DataMember(Name = "resize")] public string Resize { get; set; }
    }


    [DataContract]
    public class Sizes
    {
        [DataMember(Name = "thumb")] public Image Thumb { get; set; }
        [DataMember(Name = "medium")] public Image Medium { get; set; }
        [DataMember(Name = "large")] public Image Large { get; set; }
        [DataMember(Name = "small")] public Image Small { get; set; }
    }

    [DataContract]
    public class Medium
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "id_str")] public string IdStr { get; set; }
        [DataMember(Name = "indices")] public List<int> Indices { get; set; }
        [DataMember(Name = "media_url")] public string MediaUrl { get; set; }
        [DataMember(Name = "media_url_https")] public string MediaUrlHttps { get; set; }
        [DataMember(Name = "url")] public string Url { get; set; }
        [DataMember(Name = "display_url")] public string DisplayUrl { get; set; }
        [DataMember(Name = "expanded_url")] public string ExpandedUrl { get; set; }
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "sizes")] public Sizes Sizes { get; set; }

        [DataMember(Name = "source_status_id")]
        public long SourceStatusId { get; set; }

        [DataMember(Name = "source_status_id_str")]
        public string SourceStatusIdStr { get; set; }

        [DataMember(Name = "source_user_id")] public long SourceUserId { get; set; }

        [DataMember(Name = "source_user_id_str")]
        public string SourceUserIdStr { get; set; }

        [DataMember(Name = "video_info")] public VideoInfo VideoInfo { get; set; }
    }


    [DataContract]
    public class Entities
    {
        [DataMember(Name = "hashtags")] public List<Fragment> Hashtags { get; set; }
        [DataMember(Name = "symbols")] public List<Fragment> Symbols { get; set; }
        [DataMember(Name = "user_mentions")] public List<UserMention> UserMentions { get; set; }
        [DataMember(Name = "urls")] public List<Url> Urls { get; set; }
        [DataMember(Name = "media")] public List<Medium> Media { get; set; }
    }


    [DataContract]
    public class Entities2
    {
        [DataMember(Name = "url")] public List<Url> Url { get; set; }
        [DataMember(Name = "description")] public Description Description { get; set; }
    }


    [DataContract]
    public class Metadata
    {
        [DataMember(Name = "iso_language_code")]
        public string IsoLanguageCode { get; set; }

        [DataMember(Name = "result_type")] public string ResultType { get; set; }
    }


    [DataContract]
    public class Url
    {
        [DataMember(Name = "url")] public string UrlStr { get; set; }
        [DataMember(Name = "expanded_url")] public string ExpandedUrl { get; set; }
        [DataMember(Name = "display_url")] public string DisplayUrl { get; set; }
        [DataMember(Name = "indices")] public List<int> Indices { get; set; }
    }


    [DataContract]
    public class Description
    {
        [DataMember(Name = "urls")] public List<Url> Urls { get; set; }
    }


    [DataContract]
    public class BoundingBox
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "coordinates")] public List<List<double[]>> Coordinates { get; set; }
    }

    [DataContract]
    public class Attributes
    {
    }


    [DataContract]
    public class User
    {
        [DataMember(Name = "id")] public object Id { get; set; }
        [DataMember(Name = "id_str")] public string IdStr { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "screen_name")] public string ScreenName { get; set; }
        [DataMember(Name = "location")] public string Location { get; set; }
        [DataMember(Name = "description")] public string Description { get; set; }
        [DataMember(Name = "url")] public string Url { get; set; }
        [DataMember(Name = "entities")] public Entities2 Entities { get; set; }
        [DataMember(Name = "protected")] public bool? Protected { get; set; }
        [DataMember(Name = "followers_count")] public int FollowersCount { get; set; }
        [DataMember(Name = "friends_count")] public int FriendsCount { get; set; }
        [DataMember(Name = "listed_count")] public int ListedCount { get; set; }
        [DataMember(Name = "created_at")] public string CreatedAt { get; set; }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; set; }

        [DataMember(Name = "utc_offset")] public int? UtcOffset { get; set; }
        [DataMember(Name = "time_zone")] public string TimeZone { get; set; }
        [DataMember(Name = "geo_enabled")] public bool? GeoEnabled { get; set; }
        [DataMember(Name = "verified")] public bool? Verified { get; set; }
        [DataMember(Name = "statuses_count")] public int StatusesCount { get; set; }
        [DataMember(Name = "lang")] public string Lang { get; set; }

        [DataMember(Name = "contributors_enabled")]
        public bool? ContributorsEnabled { get; set; }

        [DataMember(Name = "is_translator")] public bool? IsTranslator { get; set; }

        [DataMember(Name = "is_translation_enabled")]
        public bool? IsTranslationEnabled { get; set; }

        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor { get; set; }

        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageUrl { get; set; }

        [DataMember(Name = "profile_background_image_url_https")]
        public string ProfileBackgroundImageUrlHttps { get; set; }

        [DataMember(Name = "profile_background_tile")]
        public bool? ProfileBackgroundTile { get; set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor { get; set; }

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor { get; set; }

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor { get; set; }

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor { get; set; }

        [DataMember(Name = "profile_use_background_image")]
        public bool? ProfileUseBackgroundImage { get; set; }

        [DataMember(Name = "has_extended_profile")]
        public bool? HasExtendedProfile { get; set; }

        [DataMember(Name = "default_profile")] public bool? DefaultProfile { get; set; }

        [DataMember(Name = "default_profile_image")]
        public bool? DefaultProfileImage { get; set; }

        [DataMember(Name = "following")] public bool? Following { get; set; }

        [DataMember(Name = "follow_request_sent")]
        public bool? FollowRequestSent { get; set; }

        [DataMember(Name = "notifications")] public bool? Notifications { get; set; }
        [DataMember(Name = "translator_type")] public string TranslatorType { get; set; }

        [DataMember(Name = "profile_banner_url")]
        public string ProfileBannerUrl { get; set; }
    }


    [DataContract]
    public class Fragment
    {
        [DataMember(Name = "text")] public string Text { get; set; }
        [DataMember(Name = "indices")] public int[] Indices { get; set; }
    }

    [DataContract]
    public class UserMention
    {
        [DataMember(Name = "screen_name")] public string ScreenName { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "id_str")] public string IdStr { get; set; }
        [DataMember(Name = "indices")] public List<int> Indices { get; set; }
    }


    [DataContract]
    public class Place
    {
        [DataMember(Name = "id")] public string Id { get; set; }
        [DataMember(Name = "url")] public string Url { get; set; }
        [DataMember(Name = "place_type")] public string PlaceType { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "full_name")] public string FullName { get; set; }
        [DataMember(Name = "country_code")] public string CountryCode { get; set; }
        [DataMember(Name = "country")] public string Country { get; set; }

        [DataMember(Name = "contained_within")]
        public List<object> ContainedWithin { get; set; }

        [DataMember(Name = "bounding_box")] public BoundingBox BoundingBox { get; set; }
        [DataMember(Name = "attributes")] public Attributes Attributes { get; set; }
    }


    [DataContract]
    public class ExtendedEntities
    {
        [DataMember(Name = "media")] public List<Medium> Media { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember(Name = "created_at")] public string CreatedAt { get; set; }
        [DataMember(Name = "id")] public object Id { get; set; }
        [DataMember(Name = "id_str")] public string IdStr { get; set; }
        [DataMember(Name = "text")] public string Text { get; set; }
        [DataMember(Name = "truncated")] public bool? Truncated { get; set; }
        [DataMember(Name = "entities")] public Entities Entities { get; set; }
        [DataMember(Name = "metadata")] public Metadata Metadata { get; set; }
        [DataMember(Name = "source")] public string Source { get; set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_status_id_str")]
        public string InReplyToStatusIdStr { get; set; }

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [DataMember(Name = "in_reply_to_user_id_str")]
        public string InReplyToUserIdStr { get; set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        [DataMember(Name = "user")] public User User { get; set; }
        [DataMember(Name = "geo")] public Coordinates Geo { get; set; }
        [DataMember(Name = "coordinates")] public Coordinates Coordinates { get; set; }
        [DataMember(Name = "place")] public Place Place { get; set; }
        [DataMember(Name = "contributors")] public object Contributors { get; set; }
        [DataMember(Name = "is_quote_status")] public bool? IsQuoteStatus { get; set; }

        [DataMember(Name = "quoted_status_id")]
        public long? QuotedStatusId { get; set; }

        [DataMember(Name = "quoted_status_id_str")]
        public string QuotedStatusIdStr { get; set; }

        [DataMember(Name = "quoted_status")] public Status QuotedStatus { get; set; }
        [DataMember(Name = "retweet_count")] public int RetweetCount { get; set; }
        [DataMember(Name = "favorite_count")] public int FavoriteCount { get; set; }
        [DataMember(Name = "favorited")] public bool? Favorited { get; set; }
        [DataMember(Name = "retweeted")] public bool? Retweeted { get; set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool? PossiblySensitive { get; set; }

        [DataMember(Name = "lang")] public string Lang { get; set; }

        [DataMember(Name = "extended_entities")]
        public ExtendedEntities ExtendedEntities { get; set; }
    }


    [DataContract]
    public class Variant
    {
        [DataMember(Name = "bitrate")] public int Bitrate { get; set; }
        [DataMember(Name = "content_type")] public string ContentType { get; set; }
        [DataMember(Name = "url")] public string Url { get; set; }
    }

    [DataContract]
    public class VideoInfo
    {
        [DataMember(Name = "aspect_ratio")] public List<int> AspectRatio { get; set; }
        [DataMember(Name = "variants")] public List<Variant> Variants { get; set; }
    }

    [DataContract]
    public class Coordinates
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public double?[] Coord { get; set; }
    }


    [DataContract]
    public class Doc
    {
        [DataMember(Name = "_id")] public string PrivateId { get; set; }
        [DataMember(Name = "_rev")] public string Rev { get; set; }
        [DataMember(Name = "created_at")] public string CreatedAt { get; set; }
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "id_str")] public string IdStr { get; set; }
        [DataMember(Name = "text")] public string Text { get; set; }
        [DataMember(Name = "truncated")] public bool? Truncated { get; set; }
        [DataMember(Name = "entities")] public Entities Entities { get; set; }
        [DataMember(Name = "metadata")] public Metadata Metadata { get; set; }
        [DataMember(Name = "source")] public string Source { get; set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_status_id_str")]
        public string InReplyToStatusIdStr { get; set; }

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [DataMember(Name = "in_reply_to_user_id_str")]
        public string InReplyToUserIdStr { get; set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        [DataMember(Name = "user")] public User User { get; set; }
        [DataMember(Name = "geo")] public Coordinates Geo { get; set; }
        [DataMember(Name = "coordinates")] public Coordinates Coordinates { get; set; }
        [DataMember(Name = "place")] public Place Place { get; set; }
        [DataMember(Name = "contributors")] public object Contributors { get; set; }

        [DataMember(Name = "retweeted_status")]
        public Status RetweetedStatus { get; set; }

        [DataMember(Name = "is_quote_status")] public bool? IsQuoteStatus { get; set; }

        [DataMember(Name = "quoted_status_id")]
        public long? QuotedStatusId { get; set; }

        [DataMember(Name = "quoted_status_id_str")]
        public string QuotedStatusIdStr { get; set; }

        [DataMember(Name = "retweet_count")] public int RetweetCount { get; set; }
        [DataMember(Name = "favorite_count")] public int FavoriteCount { get; set; }
        [DataMember(Name = "favorited")] public bool? Favorited { get; set; }
        [DataMember(Name = "retweeted")] public bool? Retweeted { get; set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool? PossiblySensitive { get; set; }

        [DataMember(Name = "lang")] public string Lang { get; set; }
        [DataMember(Name = "location")] public string Location { get; set; }

        [DataMember(Name = "extended_entities")]
        public ExtendedEntities ExtendedEntities { get; set; }
    }


    [DataContract]
    public class UniTwitterRow
    {
        [DataMember(Name = "id")] public string Id { get; set; }
        [DataMember(Name = "key")] public List<string> Key { get; set; }
        [DataMember(Name = "value")] public int Value { get; set; }
        [DataMember(Name = "doc")] public Doc Doc { get; set; }
    }
}