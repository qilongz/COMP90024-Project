from pyspark import SparkConf, SparkContext, SQLContext
from pyspark.sql import SparkSession
from pyspark.sql import Row
from pyspark.sql.types import *
from pyspark.sql.utils import AnalysisException


from pywebhdfs.webhdfs import PyWebHdfsClient
from pywebhdfs.errors import FileNotFound

from datetime import datetime
from dateutil import tz
import json 
import sys


spark = (SparkSession
	        .builder
	        .appName('Data collection with hive')
	        .enableHiveSupport()
			.getOrCreate())

sc = spark.sparkContext



def get_files(file_status):
	files = []
	if file_status == []:
		return None

	else:
		for file in file_status:
			if file['type'] == 'FILE':
				files.append(file['pathSuffix'])

		return list(set(files))



def timeCov(utc_time,country,city):
	from_zone = tz.gettz('UTC')
	to_zone = tz.gettz(country +'/'+ city)
	utc = datetime.strptime(utc_time, '%a %b %d %H:%M:%S %z %Y')
	utc = utc.replace(tzinfo=from_zone)
	mel = utc.astimezone(to_zone)
	mel_time = mel.strftime('%a %b %d %H:%M:%S %z %Y')

	return mel_time

#def remove_links(text):
#@screen name


#['created_at',  *'entities', 'geo', 'id', *'place',  'text',  *'user']
def collect_data(tweet):
	if tweet.get('geo') == None or tweet.get('geo').get('coordinates') == None or tweet.get('id') == None or tweet.get('created_at') == None:
		return None
	else:
		try:
			city = tweet['place']['name']
			country = tweet['place']['country']
			user = tweet['user']['id']
		except TypeError:
			city = None
			country = None
			user = None
			
		tweet_info = Row(city = city, country = country, created_at = tweet['created_at'], 
							geo = tweet['geo']['coordinates'], id = tweet['id'], lang = tweet['lang'],
							text = tweet['text'], user = user)
		return tweet_info


if __name__ == "__main__":
	# set up spark env
	spark = (SparkSession
	        .builder
	        .appName('Data collection with hive')
	        .enableHiveSupport()
			.getOrCreate())

	sc = spark.sparkContext


	# retrieve collected file info
	hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')

	# data_dir = '/team40/stream_data'
	data_dir = 	str(sys.argv[1])
	dump_limit = 500
	dir_status = hdfs.list_dir(data_dir)
	files = get_files(dir_status['FileStatuses']['FileStatus'])

	if files == None:
		print('No data available, please try again!')
	else:
		# logging progress
		my_log = str(sys.argv[2]) + '/file_log.txt'

		try:
			# log exist, not first time to go through the file list
			# need to ignore the files that have been read
			hdfs.get_file_dir_status(my_log)
			logged_files = sc.textFile(my_log).map(lambda x: x.strip()).collect()
			print('----------------------------------- check directory, unlogged files ----------------------------------------------------------')
			unlogged_files = [ file for file in files if file not in logged_files ]

		except FileNotFound:
			tem_log = ""
			hdfs.create_file(my_log,tem_log)
			unlogged_files = files




		print('------------------------------------------------- collection start------------------------------------------------------------')
		print(unlogged_files)
		fields = [StructField('city',StringType(),True), 
					StructField('country',StringType(),True), 
					StructField('created_at',StringType(),False),
					StructField('geo',ArrayType(DoubleType(),False),False), 
					StructField('id',LongType(),False),
					StructField('leng',StringType(),True),
					StructField('text',StringType(),True),
					StructField('user',LongType(),True)]


		df_schema = StructType(fields)
		acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)

		acc_num = 0
		for file in unlogged_files:
			new_log = file + '\n'
			my_file = data_dir + '/' + file

			twts = sc.textFile(my_file)
			parsed_twts = twts.map(json.loads)
			data = parsed_twts.map(collect_data).filter(lambda x: x != None).collect()
			if data != []:
				df = spark.createDataFrame(data, schema = df_schema)

				print('---------------------------------- file ' +  file + ' count ' + str(df.count()) + ' -----------------------------------')

				acc_df = acc_df.union(df).dropDuplicates(['id'])

				print('-------------------------------------- Current total count ' + str(acc_df.count()) +' ------------------------------------------')
			print('---------------------------------------------------- '+ file + ' done-----------------------------------------------------------')
			hdfs.append_file(my_log,new_log)

			# For every 500 records or the end of collecting on the target directory, data will be dumped into a hive table
			if acc_df.count() >= dump_limit:
				acc_num = acc_num + acc_df.count()
				print('----------------------------------------- Tweets dumped: ' + str(acc_df.count()) + ' -------------------------------------------')
				try:
					acc_df.write.saveAsTable('tem_01')
					acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)
				except AnalysisException:
					acc_df.write.mode('append').saveAsTable('tem_01')
					acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)
				print('---------------------------------- Re-initialize dataframe: ' + str(acc_df.count()) + ' ----------------------------------------')


		acc_num = acc_num + acc_df.count()
		print('-------------------------------------------- Total tweets dumped: ' + str(acc_num) + ' -----------------------------------------')		
		try:
			acc_df.write.saveAsTable('tem_01')
		except AnalysisException:
			acc_df.write.mode('append').saveAsTable('tem_01')

		df_total = spark.sql('SELECT * FROM tem_01').dropDuplicates(['id'])
		print('--------------------------- Total collected: ' + str(df_total.count()) + ' from ' + data_dir + ' -------------------------------')	
		df_total.write.saveAsTable('tweets_01')
		spark.sql('DROP TABLE IF EXISTS tem_01')






## point in polygon
## c#
## text pre-process
## time zone test
## geo -count
## sentiment analysis calculation


