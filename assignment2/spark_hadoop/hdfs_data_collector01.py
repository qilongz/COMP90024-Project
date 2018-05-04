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

#SparkContext.setSystemProperty("hive.metastore.uris", "thrift://nn1:9083")

spark = (SparkSession
	        .builder
	        .appName('Data collection with hive')
	        .enableHiveSupport()
			.getOrCreate())

sc = spark.sparkContext

# conf = SparkConf().setAppName("Data collection - Pyspark")       #setMaster("yarn")
# conf = conf.set("spark.hadoop.yarn.resourcemanager.address", "115.146.86.158:8088")
# sc = SparkContext(conf=conf)
# sqlContext = SQLContext(sc)





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


#['created_at',  *'entities', 'geo', 'id',  'lang', *'place',  'text',  *'user']
def collect_data(tweet):
	if tweet['geo'] == None:
		return None
	else:
		try:
			city = tweet['place']['name']
			country = tweet['place']['country']
			user = tweet['user']['id']
		except KeyError:
			city = None
			country = None
			user = None
			
		tweet_info = Row(twt_id = tweet['id'], user = user, geo = tweet['geo']['coordinates'], 
						city = city, country = country, created_at = tweet['created_at'], lang = tweet['lang'], text = tweet['text'])
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
	data_dir = '/user/nikikiq/spark_test2'

	dir_status = hdfs.list_dir(data_dir)
	files = get_files(dir_status['FileStatuses']['FileStatus'])

	if files == None:
		print('No data available, please try again!')
	else:
		# logging progress
		print(files)
		my_log = '/user/nikikiq/spark_test/collections/file_log.txt'

		try:
			# log exist, not first time to go through the file list
			# need to ignore the files that have been read
			hdfs.get_file_dir_status(my_log)
			logged_files = sc.textFile(my_log).map(lambda x: x.strip()).collect()
			print('----------------------------------------------------check--directory--------------------------------------------------------')
			print(logged_files)
			unlogged_files = [ file for file in files if file not in logged_files ]

		except FileNotFound:
			tem_log = ""
			hdfs.create_file(my_log,tem_log)
			unlogged_files = files




		print('-----------------------------------------------collection-start-------------------------------------------------------------')
		print(unlogged_files)
		fields = [StructField('city',StringType(),True), StructField('country',StringType(),True), StructField('created_at',StringType(),False),
					StructField('geo',ArrayType(DoubleType(),False),False), StructField('lang',StringType(),True), StructField('text',StringType(),True),
					StructField('twt_id',LongType(),False), StructField('user',LongType(),True)]
		df_schema = StructType(fields)
		acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)

		for file in unlogged_files:
			new_log = file + '\n'
			my_file = data_dir + '/' + file

			twts = sc.textFile(my_file)
			parsed_twts = twts.map(json.loads)
			data = parsed_twts.map(collect_data).filter(lambda x: x != None).collect()
			if data != []:
				df = spark.createDataFrame(data)

				print('-------------------------------------append '+ str(df.count()) + ' -----------------------------------------------')
				print(df.head(5))
				acc_df = acc_df.union(df).dropDuplicates(['twt_id'])

				print(acc_df.head(5))
				print('--------------------------------------count ' + str(acc_df.count()) +' ---------------------------------------')
			print('------------------------------------------- '+ file + ' done-------------------------------------------------------')
			
			hdfs.append_file(my_log,new_log)

		print('Total tweets:' + str(acc_df.count()))
		try:
			acc_df.write.saveAsTable('twt_02')
		except AnalysisException:
			acc_df.write.mode('append').saveAsTable('twt_02')



# StructType(List(
# 	StructField(city,StringType,true),
# 	StructField(country,StringType,true),
# 	StructField(created_at,StringType,true),
# 	StructField(geo,ArrayType(DoubleType,true),true),
# 	StructField(lang,StringType,true),
# 	StructField(text,StringType,true),
# 	StructField(id,LongType,true),
# 	StructField(user,LongType,true)))



## point in polygon
## c#
## text pre-process
## time zone test
## geo -count
## sentiment analysis calculation


