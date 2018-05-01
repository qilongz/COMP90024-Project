# To run this code, first edit config.py with your configuration, then:
#
# mkdir data
# python twitter_stream.py -q apple -d data
# 
# It will produce the list of tweets for the query "apple" 
# in the file data/stream_apple.json
import hdfs
from hdfs import InsecureClient
import tweepy
from tweepy import Stream
from tweepy import OAuthHandler
from tweepy.streaming import StreamListener
import datetime
import argparse
import string
import json
import config


def get_parser():
	"""Get parser for command line arguments."""
	parser = argparse.ArgumentParser(description="Twitter Downloader")
	parser.add_argument("-q",
						"--query",
						dest="query",
						help="Query/Filter",
						default='*')
	return parser


class MyListener(StreamListener):
	"""Custom StreamListener for streaming data."""
	
	def __init__(self, query):
		query_fname = format_filename(query)
		self.outfile = "stream_%s.json" % (query_fname)
		self.count = 0

	
	def on_data(self, data):
		try:
			if self.count <= 1000000:       
				with open(self.outfile, 'a+') as f:
					f.write(data)
				self.count += len(data)
				return True
			else:
				hdfs_path =  '/team40/stream_data/'+datetime.datetime.now().strftime("%y-%m-%d-%H-%M-%S-") + self.outfile
				client = InsecureClient('http://115.146.86.32:50070', user='qilongz')
				client.upload(hdfs_path, self.outfile)
				print(client.status(hdfs_path, strict=False))
				self.count = 0
				with open(self.outfile, 'w') as f:
					f.write(data)
				self.count += len(data)

		except BaseException as e:
			print("Error on_data: %s" % str(e))
		return True
	
	def on_error(self, status):
		print(status)
		return True


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


if __name__ == '__main__':
	parser = get_parser()
	args = parser.parse_args()
	auth = OAuthHandler(config.consumer_key, config.consumer_secret)
	auth.set_access_token(config.access_token, config.access_secret)
	api = tweepy.API(auth)
	twitter_stream = Stream(auth, MyListener(args.query))
	twitter_stream.filter(track=args.query,locations = config.ausCoordinates)
