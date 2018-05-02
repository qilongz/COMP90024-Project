import tweepy
from tweepy import OAuthHandler
import json
import logging
import string
import argparse
import time
import hdfs
from hdfs import InsecureClient
import search_config

def get_parser():
	"""Get parser for command line arguments."""
	parser = argparse.ArgumentParser(description="Twitter Searcher")
	parser.add_argument("-q",
						"--query",
						dest="query",
						help="Query/Filter",
						default='*')
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
		return '*'

def search_machine(ID,machine):
	consumer_key = machine['consumer_key']
	consumer_secret = machine['consumer_secret']
	access_token =  machine['access_token']
	access_secret =  machine['access_secret']
	auth = OAuthHandler(consumer_key, consumer_secret)
	auth.set_access_token(access_token, access_secret)
	api = tweepy.API(auth, wait_on_rate_limit_notify=True)

	"""Search for tweets via Twitter Search API."""
	sinceId = None
	max_id = ID
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
						json.dump(tweet._json,f,ensure_ascii=False)
						f.write('\n')
				tweetsCounts += len(new_tweets)
				max_id = new_tweets[-1].id
			except tweepy.RateLimitError as e:
				print(machine['index'],'Time to sleep 15 mins') 
				API_status[machine['index']] = False
				if machine['index'] == 0:
					API_status['time'] = time.time() + 901.00
				return finshed_job,max_id
			except tweepy.TweepError as e:
				logging.error(str(e))
				break
	f.close()
	return finshed_job,max_id
	
def upload_hdfs(outfile):
	try :
		destination_dir = '/team40/3_search_data/'+ time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + outfile
		hdfs = InsecureClient('http://115.146.86.32:50070', user='qilongz')
		hdfs.upload(destination_dir, outfile)
	except Exception as e:
		logging.error(str(e))\

if __name__ == '__main__':
	parser = get_parser()
	args = parser.parse_args()
	finshed_job  = False
	maxID = 989051025540730881
	geo = search_config.Geocode
	query = args.query
	outfile = "_4_search_%s.json" % (args.query)	
	searchLimits = 100
	maxTweets = 1000000
	query_fname = format_filename(query)
	API_status = {'machine1':True,'machine2':True,'machine3':True,'machine4':True,'time':0.0}
	job_record = ''
	while finshed_job == False:
		if API_status['machine1'] == True:
			print('working with 1')
			s = time.time()
			finshed_job,maxID = search_machine(maxID,search_config.machine1)
			job_record += time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + '\t' + str(maxID) +'\n'
			upload_hdfs(outfile)
			e = time.time()
			print ('time used',e-s)
		if API_status['machine2'] == True:
			print('working with 2')
			s = time.time()
			finshed_job,maxID = search_machine(maxID,search_config.machine2)
			job_record += time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + '\t' + str(maxID) +'\n'
			upload_hdfs(outfile)
			e = time.time()
			print ('time used',e-s)
		if API_status['machine3'] == True:
			print('working with 3')
			s = time.time()
			finshed_job,maxID = search_machine(maxID,search_config.machine3)
			job_record += time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + '\t' + str(maxID) +'\n'
			upload_hdfs(outfile)
			e = time.time()
			print ('time used',e-s)
		if API_status['machine4'] == True:
			print('working with 4')
			s = time.time()
			finshed_job,maxID = search_machine(maxID,search_config.machine4)
			job_record += time.strftime('%Y-%m-%d_%H-%M',time.localtime()) + '\t' + str(maxID) +'\n'
			upload_hdfs(outfile)
			e = time.time()
			print ('time used',e-s)

		with open('job_record.txt','a+') as f:
			print(job_record,file = f )
			job_record = ''
		
		time_to_wait  = API_status['time'] -time.time()
		
		if  time_to_wait >= 0.0:
			time.sleep(time_to_wait)
			API_status = {'machine1':True,'machine2':True,'machine3':True,'machine4':True,'time':0.0}
		else:
			API_status = {'machine1':True,'machine2':True,'machine3':True,'machine4':True,'time':0.0}


		
	