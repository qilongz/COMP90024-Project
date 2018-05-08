from pywebhdfs.webhdfs import PyWebHdfsClient
from pyspark import SparkConf, SparkContext, SQLContext
from pywebhdfs.errors import FileNotFound
#from pyspark.sql import SparkSession
import pandas as pd
from datetime import datetime
from dateutil import tz
import os


# not working ????
os.environ["SPARK_HOME"] = "/usr/hdp/current/spark2-client/"
os.environ["PYSPARK_PYTHON"]="python3"


# create Spark context with Spark configuration
conf = SparkConf().setAppName("Data collection - Pyspark")       #setMaster("yarn")
conf = conf.set("spark.hadoop.yarn.resourcemanager.address", "115.146.86.158:8088")
sc = SparkContext(conf=conf)
sqlContext = SQLContext(sc)

def get_files(file_status,data_dir):
	files = []
	if file_status == []:
		return None

	else:
		for file in file_status:
			if file['type'] == 'FILE':
				files.append(data_dir + '/' + file['pathSuffix'])

		return list(set(files))

def timeCov(utc_time):
	from_zone = tz.gettz('UTC')
	to_zone = tz.gettz('Australia/Melbourne')
	utc = datetime.strptime(utc_time, '%a %b %d %H:%M:%S %z %Y')
	utc = utc.replace(tzinfo=from_zone)
	mel = utc.astimezone(to_zone)
	mel_time = mel.strftime('%a %b %d %H:%M:%S %z %Y')

	return mel_time

def getGeo(tuple):
	if isinstance(tuple[0],str):
		coord = (tuple[1][0],tuple[1][1])
	else:
		coord = (tuple[0][0],tuple[0][1])
	return coord

#['created_at',  *'entities', 'geo', 'id',  'lang', *'place',  'text',  *'user']

def tweets_processor(file):
	df = sqlContext.read.json(file).distinct().filter('geo is not null').select('id','created_at', 'geo','lang','text')
	if df.count() != 0:
		pd_df = df.toPandas()
		pd_df['geo'] = pd_df['geo'].apply(getGeo)
		pd_df['created_at'] = pd_df['created_at'].apply(timeCov)

		return pd_df

	#.filter('geo is not null')
	#.select('id', 'created_at', 'geo','place','lang','text','user' )

	

	# 
	# cc = df_rdd.count()
	# rdd.map(lambda row: (row.id, row.geo, row.created_at, row.entities, row.lang, row.text, row.user, row.place))
	# new_df = sqlContext.createDataFrame(df_rdd)
	# print(df_unique.schema.names)
	# print("")

# def customFunction(row):

#    return (row.name, row.age, row.city)

# sample2 = sample.rdd.map(customFunction)
# sample2 = sample.rdd.map(lambda x: (x.name, x.age, x.city))

if __name__ == "__main__":
# 	settings
	# spark = SparkSession \
	# 	.builder \
	# 	.appName("Josn Test") \
	# 	.config("spark.hadoop.yarn.resourcemanager.address", "115.146.86.158:8088") \
	# 	.getOrCreate() 

	# read in file paths
	hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
	# data_dir = '/team40/stream_data'
	data_dir = '/user/nikikiq/spark_test'
	dir_status = hdfs.list_dir(data_dir)
	files = get_files(dir_status['FileStatuses']['FileStatus'],data_dir)

	if files == None:
		print('No data available, please try again!')
	else:
		# # create log for files have been processed
		log_data = '\n'.join(files) +'\n'
		# my_log = '/team40/collections/file_log.txt'
		my_log = '/user/nikikiq/spark_test/collections/file_log.txt'
		hdfs.create_file(my_log, log_data)

		# # start processing
		# my_collection = '/team40/collections/twitterStream.json'
		my_collection = '/user/nikikiq/spark_test/collections/twitterStream.json'
		try:
			hdfs.get_file_dir_status(my_collection)

		except FileNotFound:
			new_data = ""
			hdfs.create_file(my_collection, new_data)

		dfs = list(map(tweets_processor,files))
		big_df = pd.concat(dfs)
		big_df = big_df.drop_duplicates(subset = ['id'], keep = 'first')
		my_data = big_df.to_json(orient='records')[1:-1].replace('},{', '}\n{') + '\n'
		hdfs.append_file(my_collection, my_data)	
		#append	
		print("Data pre-procesing for " + str(len(files)) + " stream data files are done!")






		# df = sqlContext.read.json(files[0]).toPandas()
		# df = df.loc[df['geo'] != None]
		# print(df)
		# new_df = tweets_processor(files[0])
		# pandas_df = new_df.toPandas()
		# data = pandas_df.to_json(orient='records')[1:-1].replace('},{', '} {')
		# my_file = 'user/nikikiq/write/test.json'
		# hdfs.create_file(my_file, data)
		







#     Tweets with a specific latitude/longitude “Point” coordinate
#     Tweets with a Twitter “Place” 

# Tweets with a Point coordinate come from GPS enabled devices, and represent the exact GPS location of the Tweet in question. 
# This type of location does not contain any contextual information about the GPS location being referenced 
# (e.g. associated city, country, etc.), unless the exact location can be associated with a Twitter Place.

# Note that the “coordinates” attributes is formatted as [LONGITUDE, latitude], while the “geo” attribute is formatted as [latitude, LONGITUDE].

# Tweets with a Twitter “Place” contain a polygon, consisting of 4 lon-lat coordinates that define the general area (the “Place”) from which 
# the user is posting the Tweet. Additionally, the Place will have a display name, type (e.g. city, neighborhood), and country code corresponding 
# to the country where the Place is located, among other fields.

# Important note: Retweets can not have a Place attached to them, so if you use an operator such as has:geo, you will not match any Retweets


