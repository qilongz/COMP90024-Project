import tweepy #https://github.com/tweepy/tweepy
import csv
import config
from tweepy import OAuthHandler
import logging
import time
import hdfs
from hdfs import InsecureClient
import json
import itertools
import argparse
#Twitter API credentials

def get_parser():
	"""Get parser for command line arguments."""
	parser = argparse.ArgumentParser(description="Twitter Searcher")
	parser.add_argument("-f",
						"--userfile",
						dest="userCSV",
						help="userCSV/Filter",
						default='*.csv')
	return parser

def upload_hdfs(outfile):
	# Upload temp file to hdfs
	try :
		destination_dir = '/team40/'  + 'user_search_data/' + outfile
		hdfs = InsecureClient('http://115.146.86.32:50070', user='qilongz')
		hdfs.upload(destination_dir, outfile)
	except Exception as e:
		logging.error(str(e))

def get_all_tweets(users_list,machine):
	#Twitter only allows access to a users most recent 3240 tweets with this method
	#authorize twitter, initialize tweepy
	consumer_key = machine['consumer_key']
	consumer_secret = machine['consumer_secret']
	access_token =  machine['access_token']
	access_secret =  machine['access_secret']
	auth = OAuthHandler(consumer_key, consumer_secret)
	auth.set_access_token(access_token, access_secret)
	api = tweepy.API(auth,wait_on_rate_limit=True ,wait_on_rate_limit_notify=True)
	with open (outfile,'w+') as writer:
		count = 0
		for i in users_list:
			
			try:
				#initialize a list to hold all the tweepy Tweets
				alltweets = []	

				#make initial request for most recent tweets (200 is the maximum allowed count)
				new_tweets = api.user_timeline(user_id =i,count=200)
				
				#save most recent tweets
				alltweets.extend(new_tweets)

				#save the id of the oldest tweet less one
				if alltweets:
					oldest = alltweets[-1].id - 1
				else:
					oldest = -1
				
				#keep grabbing tweets until there are no tweets left to grab
				while len(new_tweets) > 0:
					
					#all subsiquent requests use the max_id param to prevent duplicates
					new_tweets = api.user_timeline(user_id =i,count=200,max_id=oldest)
					
					#save most recent tweets
					alltweets.extend(new_tweets)
					
					#update the id of the oldest tweet less one
					oldest = alltweets[-1].id - 1
					
					# print ("...%s tweets downloaded so far" % (len(alltweets)))

				for tweet in alltweets:
					if tweet.coordinates or tweet.place:
						json.dump(tweet._json,writer,ensure_ascii=False)
						count += 1
						writer.write('\n')
			except tweepy.TweepError as e:
				logging.error(str(e))
				pass
			except Exception as e:
				logging.error(str(e))
				pass
			print ("...%s tweets downloaded so far" % (count))
		


if __name__ == '__main__':
	#pass in the username of the account you want to download
	parser = get_parser()
	args = parser.parse_args()
	userFile = args.userCSV
	outfile ="user_search.json" 
	userID_list = []
	print (userFile)
	with open(userFile) as f:
		reader = csv.reader(f)
		for row in reader:
			if row:
				userID_list.append(row[0])

	userID_chunks  = [userID_list[i:i+100] for i in range(0, len(userID_list), 500)]
	for i in  userID_chunks:
		get_all_tweets(i,config.machine1)
		upload_hdfs(outfile)