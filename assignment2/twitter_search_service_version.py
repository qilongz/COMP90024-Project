import tweepy
from tweepy import OAuthHandler
import jsonpickle
import config
import logging
import string
import argparse
import datetime
import pywebhdfs
from pywebhdfs.webhdfs import PyWebHdfsClient
def get_parser():
    """Get parser for command line arguments."""
    parser = argparse.ArgumentParser(description="Twitter Searcher")
    parser.add_argument("-q",
                        "--query",
                        dest="query",
                        help="Query/Filter",
                        default='-')
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
    sinceId = None
    max_id = startID
    tweetsCounts  = 0
    finshed_job = False
    with open (outfile,'w+') as f:
        while tweetsCounts < maxTweets:
            try:
                if (max_id <= 0):
                    if (not sinceId):
                        new_tweets = api.search(
                            q = query,
                            geocode = geo,
                            count = searchLimits)
                    else:
                        new_tweets =  api.search(
                            q=query,
                            count = searchLimits,
                            geocode=geo,
                            sinceId = sinceId)
                else:
                    if (not sinceId):
                        new_tweets = api.search(
                            q=query, 
                            count=searchLimits,
                            geocode = geo,
                            max_id=str(max_id - 1))
                    else:
                        new_tweets = api.search(
                            q=query, 
                            count=searchLimits,
                            geocode = geo,
                            max_id=str(max_id - 1),
                            since_id=sinceId)
                if not new_tweets:
                    print("NO MORE TWEETS")
                    finshed_job = True
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
    f.close()
    return finshed_job,max_id
    

if __name__ == '__main__':
    parser = get_parser()
    args = parser.parse_args()
    auth = OAuthHandler(config.consumer_key, config.consumer_secret)
    auth.set_access_token(config.access_token, config.access_secret)
    api = tweepy.API(auth, wait_on_rate_limit=True, wait_on_rate_limit_notify=True)
    geo = config.Geocode
    query = args.query
    searchLimits = 100
    maxTweets = 10000
    query_fname = format_filename(query)
    startID = -1
    outfile = "search_%s.json" % ( query_fname)
    finshed_job = False
    hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='qilongz')
    while finshed_job == False:
        finshed_job,startID = search(api,geo,query,startID,searchLimits,maxTweets,outfile)
        destination_dir = 'team40/'+datetime.datetime.now().strftime("%y-%m-%d-%H-%M-%S-") + outfile
        with open(outfile) as f:
             hdfs.create_file(destination_dir, file_data=f)
        