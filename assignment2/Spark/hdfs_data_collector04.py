# Author: Yueni Chang 884622
# Date: 05/08/2018
# Clustering and Cloud Computing

# environment:
# export SPARK_HOME=/usr/hdp/current/spark2-client/
# export PYSPARK_PYTHON=python3
# usage:
# local: spark-submit /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>
# cluster: spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>
#
# This script is used to extract and reformat the twitter data provided by university
# ---------------------------------------------------------------------------------------------------------------------------------------
from pyspark import SparkConf, SparkContext, SQLContext
from pyspark.sql import SparkSession
from pyspark.sql import Row
from pyspark.sql.types import *
from pyspark.sql.utils import AnalysisException

from json.decoder import JSONDecodeError

from pywebhdfs.webhdfs import PyWebHdfsClient
from pywebhdfs.errors import FileNotFound

import json 
import sys

# get all file names in a given dir
def get_files(file_status):
	files = []
	if file_status == []:
		return None

	else:
		for file in file_status:
			if file['type'] == 'FILE':
				files.append(file['pathSuffix'])

		return list(set(files))


# extract information from tweets data and reformat the data for further process
# only 8 fileds are extracted from tweets:
# city, country, created_at, geo, id (tweet id), langangue code, text, user_id
# to keep consistency in the analysis, all the records wiouth geo location are discarded 
def collect_data(tweet):
	twt_id = int(tweet['id'])
	twt = tweet['doc']
	if twt['geo'] == None or twt['id'] == None or twt['created_at'] == None:
		return None
	else:
		geo = [float(coord) for coord in twt['geo']['coordinates'] ]

		try:
			city = tweet['key'][0].title()
			if city == 'Perth':
				city = 'Perth (WA)'
			country = 'Australia'
			user = twt['user']['id']
		except TypeError:
			city = None
			country = None
			user = None
			
		tweet_info = Row(city = city, country = country, created_at = twt['created_at'], 
							geo = geo, id = twt_id, lang = twt['lang'],
							text = twt['text'], user = user)
		return tweet_info

def reformat(line):
	if 'Point' in line:
		line = line.strip()
		if line[0] == '[':
			line = line[1:-1]
		else:
			line = line[0:-1]
		return line
	else:
		return None



if __name__ == "__main__":

	# set up spark env
	spark = (SparkSession
	        .builder
	        .appName('Data collection with hive')
	        .enableHiveSupport()
			.getOrCreate())

	sc = spark.sparkContext



	hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
	data_dir = 	str(sys.argv[1])
	dump_limit = 2000
	dir_status = hdfs.list_dir(data_dir)
	files = get_files(dir_status['FileStatuses']['FileStatus'])

	if files == None:
		print('No data available, please try again!')
	else:
		# checked logged files and files which may fail due to various errors
		try:
			with open('/vdc/team40/nia_test/collected/file_log.txt','r') as my_log:
				logged_files = [line.strip() for line in my_log]
				my_log.close()

		except FileNotFoundError:
			logged_files = []

		try:
			with open('/vdc/team40/nia_test/collected/fail_log.txt','r') as my_fail:
				failed_files = [line.strip() for line in my_fail]
				my_fail.close()
		except FileNotFoundError:
			failed_files = []

		history = logged_files + failed_files
		unlogged_files = [ file for file in files if file not in history ]

		print('----------------------------------- check directory, logged files ----------------------------------------------------------')
		print(logged_files)
		print('------------------------------------------------- collection start on ------------------------------------------------------')
		print(unlogged_files)


		fields = [StructField('city',StringType(),True), 
					StructField('country',StringType(),True), 
					StructField('created_at',StringType(),False),
					StructField('geo',ArrayType(DoubleType(),False),False), 
					StructField('id',LongType(),False),
					StructField('lang',StringType(),True),
					StructField('text',StringType(),True),
					StructField('user',LongType(),True)]


		df_schema = StructType(fields)
		acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)
		acc_num = 0
		new_log = ''



		for file in unlogged_files:
			my_file = data_dir + '/' + file
			print('--------------------------------------------- Current file ' +  file +'---------------------------------------------------------')
			twts = sc.textFile(my_file).map(reformat).filter(lambda x: x != None)				
			parsed_twts = twts.map(json.loads)
			data = parsed_twts.map(collect_data).filter(lambda x: x != None).collect()
			print('---------------------------------------------------------------------------------------------------------------------------------')
			print(data)

			twts.unpersist()
			parsed_twts.unpersist()


			if data != []:
				df = spark.createDataFrame(data, schema = df_schema)

				print('---------------------------------- file ' +  file + ' count ' + str(df.count()) + ' -----------------------------------')

				acc_df = acc_df.union(df).dropDuplicates(['id'])
				current_total = acc_df.count()
				print('-------------------------------------- Current total count ' + str(current_total) +' ------------------------------------------')
			print('---------------------------------------------------- '+ file + ' done-----------------------------------------------------------')
			new_log = new_log + file + '\n'
			

			# For every 200 records or the end of collecting on the target directory, data will be dumped into a hive table
			# used rdd will be free up

			if current_total >= dump_limit:
				acc_num = acc_num + current_total
				print('----------------------------------------- Tweets dump prepare : ' + str(current_total) + ' -------------------------------------------')
				try:
					acc_df.write.saveAsTable('uni_tem')
				except AnalysisException:
					acc_df.write.mode('append').saveAsTable('uni_tem')


				acc_df = spark.createDataFrame(sc.emptyRDD(), schema = df_schema)
				print('----------------------------------------- Tweets dump successful, logging files   -------------------------------------------')

				with open('/vdc/team40/nia_test/collected/file_log.txt','a') as my_log:
					my_log.write(new_log)
					my_log.close()

				new_log = ''
				print('---------------------------------- Re-initialize dataframe: ' + str(acc_df.count()) + ' ----------------------------------------')
			

		acc_num = acc_num + acc_df.count()
		print('-------------------------------------------- Total tweets dumped: ' + str(acc_num) + ' -----------------------------------------')		
		try:
			acc_df.write.saveAsTable('uni_tem')
		except AnalysisException:
			acc_df.write.mode('append').saveAsTable('uni_tem')


		with open('/vdc/team40/nia_test/collected/file_log.txt','a') as my_log:
			my_log.write(new_log)
			my_log.close()

		# finish up final check
		df_total = spark.sql('SELECT * FROM uni_tem').dropDuplicates(['id'])
		print('--------------------------- Total collected: ' + str(df_total.count()) + ' from ' + data_dir + ' -------------------------------')	
		df_total.write.saveAsTable(str(sys.argv[2]))
		spark.sql('DROP TABLE IF EXISTS uni_tem')





