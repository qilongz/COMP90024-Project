import tweepy  
consumer_key = "S8bfm3C7oxLr5SEzwR6XAgWNH"  
consumer_secret = "w9OmX4TdHRUmXJoDcpYr9c1jle5DVZ38QHPEClBuBjbdDJQdSp"  
access_token = "988017286643564544-ygFPpACyDWvUPQpGpoRJkCupLUNsHpD"  
access_token_secret = "	lKQekCUtKKWKv5I8F6sZkwAxSNOKBJekKPnmSyCnka07c"


# Creating the authentication object
auth = tweepy.OAuthHandler(consumer_key, consumer_secret)
# Setting your access token and secret
auth.set_access_token(access_token, access_token_secret)
# Creating the API object while passing in auth information
api = tweepy.API(auth) 


# Using the API object to get tweets from your timeline, and storing it in a variable called public_tweets
public_tweets = api.home_timeline()
# foreach through all tweets pulled
for tweet in public_tweets:
   # printing the text stored inside the tweet object
   print (tweet.text)