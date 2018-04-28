import tweepy
from tweepy import OAuthHandler
import json 
import config
import logging
import string
import argparse
import time

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

def search(api, geo, query,limit,outfile):
    """Search for tweets via Twitter Search API."""
    # Track the upper and lower bound of each returned set.
    lower_id = None
    upper_id = -1

    # Track number of tweets returned in total.
    tweet_count = 0

    # Pull tweets until erorr or no more to process.
    while True:
        try:
            if (upper_id <= 0):
                if (not lower_id):
                    new_tweets = api.search(
                        q=query,
                        geocode=geo,
                        count=limit
                    )

                else:
                    new_tweets = api.search(
                        q=query,
                        geocode=geo,
                        count=limit,
                        since_id=lower_id
                    )
            else:
                if (not lower_id):
                    new_tweets = api.search(
                        q=query,
                        geocode=geo,
                        count=limit,
                        upper_id=str(upper_id - 1)
                    )
                else:
                    new_tweets = api.search(
                        q=query,
                        geocode=geo,
                        count=limit,
                        upper_id=str(upper_id - 1),
                        since_id=lower_id
                    )

            # Exit when no new tweets are found.
            if not new_tweets:
                logging.info("No more tweets to read.")
                break

            # Process received tweets.
            for tweet in new_tweets:

                jtweet = tweet._json

                # # Only store tweets that have location we can use.
                if tweet.coordinates or tweet.place:
                    jtweet['_id'] = str(jtweet['id'])
                    # try:
                    #     self.db.save(jtweet)
                    # except couchdb.http.ResourceConflict:
                    #     logging.info("Ignored duplicate tweet.")
                    try:
                        with open(outfile, 'a+') as f:
                            json.dump(jtweet, f)
                            f.write('\n')
                    except BaseException as e:
                        print("Error on_data: %s" % str(e))
                        time.sleep(5)
            # Output current number of tweets.
            tweet_count += len(new_tweets)
            print (tweet_count)
            logging.info("Downloaded {0} tweets".format(tweet_count))

            # Track upper id.
            upper_id = new_tweets[-1].id

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
    limit = 50
    query_fname = format_filename(query)
    outfile = "%s/search_%s.json" % (args.data_dir, query_fname)
    search(api,geo, query,limit,outfile)