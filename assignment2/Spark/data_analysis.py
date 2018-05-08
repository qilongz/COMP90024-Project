# Author: Yueni Chang 884622
# Date: 05/08/2018
# Clustering and Cloud Computing

# environment:
# export SPARK_HOME=/usr/hdp/current/spark2-client/
# export PYSPARK_PYTHON=python3
# usage:
# local: spark-submit /path/to/this .py/file <table_name>  </path/to/local/dir>  
# cluster: spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file <table_name>  </path/to/local/dir>  
#
# This script is used to analyse extracted data and generate csv file for analysis result, which including tweet count and sentiment analysis score.
# --------------------------------------------------------------------------------------------------------------------------------------------------
from pyspark import SparkConf, SparkContext, SQLContext
from pyspark.sql import SparkSession
from pyspark.sql import Row
from pyspark.sql.types import *
import sys

from datetime import datetime
from dateutil import tz

import json
import re
import nltk
from nltk.sentiment.vader import SentimentIntensityAnalyzer

# to initialize with vader source package:
# nltk.download('vader_lexicon')
sid = SentimentIntensityAnalyzer()



cities = ['Sydney', 'Melbourne', 'Canberra', 'Perth (WA)', 'Hobart', 'Adelaide', 'Brisbane']

# convert time to local time for calpital cities
def timeConv(utc_time,country,city):
	from_zone = tz.gettz('UTC')
	if city == 'Adelaide':
		to_zone = tz.gettz(country +'/'+ city)
	elif city == 'Perth (WA)':
		to_zone = tz.gettz(country +'/Perth')
	else:
		to_zone = tz.gettz(country +'/Melbourne')
	utc = datetime.strptime(utc_time, '%a %b %d %H:%M:%S %z %Y')
	utc = utc.replace(tzinfo=from_zone)
	local = utc.astimezone(to_zone)
	local_time = local.strftime('%a %b %d %H:%M:%S %z %Y')

	return local, local_time

# remove links and @user-names for sentiment analysis
# since some machine learning algorithm may require pre-process
# while some other tools may not
# since some people may use hashtag's text to express their opinion as well, we keep hashtags in, but remove the hash key
# e.g TEXT: @DannySmith: I am so #exciting to see this http://shared/link  
# after process: I am so exciting to see this 
# return the processed text and corresponding sentiment score
def process_text(text):
	if text == None:
		return None, float(0)
	else:
		no_link = re.sub(r'\s(https?)?(:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=0-9]{1,256}\.[-a-zA-Z0-9@:%_\+.~#?&//=]*\b','',text)
		no_name = re.sub(r'\s\@[a-zA-Z0-9_]{1,15}','',no_link)
		score = sid.polarity_scores(no_name)
		compound_score = score['compound']
		return no_name, compound_score

# process the extracted tweets
def processor(tweet):
	if tweet['city'] not in cities:
		return None 
	else:
		if tweet['city'] == 'Perth (WA)': 
			city = 'Perth'
		else:
			city = tweet['city']
		local, local_time = timeConv(tweet['created_at'],tweet['country'],tweet['city'])
		month = str(local.month) + ' ' + str(local.year)
		hour = local.hour

		text,sscore = process_text(tweet['text'])
		
		tweet_info = Row(city = city, country = tweet['country'], created_at = local_time, day = local_time[0:3], geo = tweet['geo'], 
							hour = hour,  id = tweet['id'], lang = tweet['leng'], month = month, sentiment = sscore, 
							text = text, user = tweet['user'])
		return tweet_info



def get_area_count(spark_df):
	total_count = spark_df.rdd.map(lambda row: (row.city, 1)).reduceByKey(lambda v1,v2: v1 + v2).collect()
	return total_count


def get_area_sscore(spark_df, mode):

	avg_sent = spark_df.rdd.filter(lambda x: x.sentiment != 0.0 ) \
						.map(lambda row: (row[mode], (row.sentiment, 1))) \
						.reduceByKey(lambda p1,p2: tup_sum(p1,p2)) \
						.map(lambda p: (p[0], round(p[1][0] / p[1][1],4))) \
						.collect()

	fields = [StructField(mode,StringType(),False), 
				StructField('sentiment_score', DoubleType(),False)]
	schema = StructType(fields)
	new_df = spark.createDataFrame(avg_sent, schema = schema)
	new_pdf = new_df.toPandas()
	new_pdf.to_csv((my_dir +'/avg_'+ mode + '.csv'),header = True)
	return new_df

def tup_sum(tp1,tp2):
	v1 = tp1[0] + tp2[0]
	v2 = tp1[1] + tp2[1]
	return (v1,v2)


	 

if __name__ == "__main__":
	my_dir = sys.argv[2]
	my_table = str(sys.argv[1])

	# set up spark env
	spark = (SparkSession
	        .builder
	        .appName('Data collection with hive')
	        .enableHiveSupport()
			.getOrCreate())

	sc = spark.sparkContext

	df = spark.sql(('SELECT * FROM ' + my_table))
	processed_data = df.rdd.map(processor).filter(lambda x: x != None).collect()
# Row(city, country, created_at, day, geo, hour, id, lang, month, sentiment, text, user, year)
	fields = [StructField('city',StringType(),True), 
					StructField('country',StringType(),True), 
					StructField('created_at',StringType(),False),
					StructField('day',StringType(),False),
					StructField('geo',ArrayType(DoubleType(),False),False),
					StructField('hour',IntegerType(),False),
					StructField('id',LongType(),False),
					StructField('lang',StringType(),True),
					StructField('month',StringType(),False),
					StructField('sentiment',DoubleType(),False),
					StructField('text',StringType(),True),
					StructField('user',LongType(),True)]
	df_schema = StructType(fields)
	pro_df = spark.createDataFrame(processed_data, schema = df_schema)

	area_count = get_area_count(pro_df)
	avg_sent_by_city = get_area_sscore(pro_df,'city')
	avg_sent_by_hour = get_area_sscore(pro_df,'hour')
	avg_sent_by_day = get_area_sscore(pro_df,'day')


	pd_df = pro_df.toPandas()
	my_data = pd_df.to_json(orient='records')[1:-1].replace('},{', '}\n{') + '\n'

	with open((my_dir + '/stream_data.json'),'w') as file:
		file.write(my_data)
		file.close()
