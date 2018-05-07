import tweepy #https://github.com/tweepy/tweepy
import csv
import config
from tweepy import OAuthHandler
import logging
import time
import hdfs
from hdfs import InsecureClient
import json
import jsonpickle
import itertools
#Twitter API credentials

def write_hdfs(tweet):
	hdfs = InsecureClient('http://115.146.86.32:50070', user='qilongz')
	destination_dir = hdfs_dir + outfile
	try :
		if hdfs.status(destination_dir,strict = False) == None:
			hdfs.write(destination_dir,data = tweet)
		else:
			hdfs.write(destination_dir,data = tweet,append =True)
	except Exception as e:
		logging.error(str(e))

def upload_hdfs(outfile):
	try :
		destination_dir = '/team40/'  + 'user_search_data/'+ time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + outfile
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
		for i in users_list:
			try:
				#initialize a list to hold all the tweepy Tweets
				alltweets = []	

				#make initial request for most recent tweets (200 is the maximum allowed count)
				new_tweets = api.user_timeline(user_id =i,count=200)
				
				#save most recent tweets
				alltweets.extend(new_tweets)

				#save the id of the oldest tweet less one
				oldest = alltweets[-1].id - 1
				
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
						writer(jsonpickle.encode(tweet._json, unpicklable=False) +'\n')
			except tweepy.TweepError as e:
				logging.error(str(e))
				pass
		

if __name__ == '__main__':
	#pass in the username of the account you want to download

	outfile ="user_search.json" 
	hdfs_dir = '/team40/'+ 'user_search_data/' 
	userID_list = []
	with open('userHomeCity.csv') as f:
		reader = csv.reader(f)
		for row in reader:
			if row:
				userID_list.append(row[0])
	
	index_25 =  int(len(userID_list)*0.25)
	index_50  = int(len(userID_list)*0.50)
	index_75  = int(len(userID_list)*0.75)
	part1  = userID_list[:index_25-1]
	part2  = userID_list[index_25 :index_50-1]
	part3  = userID_list[index_50 :index_75 -1]
	part4  = userID_list[index_75 :]
	#get_all_tweets(part3,config.stream_api)

	part1_chunks  = [part1[i:i+100] for i in range(0, len(userID_list[:index_25-1]), 500)]	
	for i in  part1_chunks:
		get_all_tweets(i,config.machine1)
		upload_hdfs(outfile)
		print('YES ! loaded')

