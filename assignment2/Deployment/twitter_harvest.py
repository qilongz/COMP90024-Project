import tweepy
from tweepy import OAuthHandler
import json 
consumer_key = 'fxAkAR5lwg2Ruwp44iW4QlCdj'
consumer_secret = '0g31TlRcU2LuwabCuziFRvPQvPIvkIOUe766wbUJqiSJjDhRQ8'
access_token = '988044719287549954-skN9LAPejtC5jePxe0yK5U0LXm89mRQ'
access_secret = 'QTVWgbQUq0NKwW1UcQI0afX40coiyWr6a867lt7nBfjSK'
 
auth = OAuthHandler(consumer_key, consumer_secret)
auth.set_access_token(access_token, access_secret)
api = tweepy.API(auth)

def process_or_store(tweet):
	print ("work")
	print(json.dumps(tweet))


for status in tweepy.Cursor(api.home_timeline).items(10):
    # Process a single status
    process_or_store(status._json)