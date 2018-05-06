import tweepy
from tweepy import OAuthHandler
import json 
import jsonpickle
import config
import logging
import string
import argparse

def get_parser():
    """Get parser for command line arguments."""
    parser = argparse.ArgumentParser(description="Twitter Downloader")
    parser.add_argument("-q",
                        "--query",
                        dest="query",
                        help="Query/Filter",
                        default='-')
    parser.add_argument("-d",
                        "--data-dir",
                        dest="data_dir",
                        help="Output/Data Directory")
    return parser


def format_filename(fname):
    """Convert file name into a safe string.
    Arguments:
        fname -- the file name to convert
    Return:
        String -- converted file name
    """
    return ''.join(convert_valid(one_char) for one_char in fname)

def convert_valid(one_char):
    """Convert a character into '_' if invalid.
    Arguments:
        one_char -- the char to convert
    Return:
        Character -- converted char
    """
    valid_chars = "-_.%s%s" % (string.ascii_letters, string.digits)
    if one_char in valid_chars:
        return one_char
    else:
        return '_'

def search(api,geo,query,startID,searchLimits,maxTweets,outfile):
    """Search for tweets via Twitter Search API."""
    sinceId = startID
    max_id = -1
    tweetsCounts  = 0
    with open (outfile,'w') as f:
        while tweetsCounts < maxTweets:
            try:
                if (max_id <= 0):
                    if (not sinceId):
                        new_tweets = api.search(
                            q = query,
                            count = searchLimits)
                    else:
                        new_tweets =  api.search(
                            q=query,
                            count = searchLimits,
                            geocode=geo,
                            sinceId = sinceId)
                else:
                    if(not sinceId):
                        new_tweets=  api.search(
                            q = query,
                            geocode=geo,
                            count = searchLimits)
                if not new_tweets:
                    #print
                    break
                for tweet in new_tweets:
                    if tweet.coordinates or tweet.place:
                        f.write(jsonpickle.encode(tweet._json, unpicklable=False) +'\n')
                tweetsCounts += len(new_tweets)
                print("Downloaded {0} tweets".format(tweetsCounts))
                max_id = new_tweets[-1].id
            # Exit upon error.
            except tweepy.TweepError as e:
                logging.error(str(e))
                break


if __name__ == '__main__':
    parser = get_parser()
    args = parser.parse_args()
    auth = OAuthHandler(config.consumer_key, config.consumer_secret)
    auth.set_access_token(config.access_token, config.access_secret)
    api = tweepy.API(auth, wait_on_rate_limit=True, wait_on_rate_limit_notify=True)
    geo = config.Geocode
    query = args.query
    limit = 100
    maxTweets = 1000
    query_fname = format_filename(query)
    startID = None
    outfile = "%s/search_%s.json" % (args.data_dir, query_fname)
    search(api,geo,query,limit,startID,maxTweets,outfile)