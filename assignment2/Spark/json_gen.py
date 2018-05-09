# Author: Yueni Chang 884622
# Date: 05/08/2018
# Clustering and Cloud Computing

# environment:
# export SPARK_HOME=/usr/hdp/current/spark2-client/
# export PYSPARK_PYTHON=python3
# usage:
# local: spark-submit /path/to/this .py/file <table_name> </path/to/hdfs/file>  </path/to/local/file>  
# cluster: spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  <table_name> </path/to/hdfs/file>  </path/to/local/file> 
#
# This script is used to generate a JSON file, and then one copy will be stored in hdfs and another copy will be write into local directory 
# ---------------------------------------------------------------------------------------------------------------------------------------
from pyspark import SparkContext
from pyspark.sql import SparkSession
from pywebhdfs.webhdfs import PyWebHdfsClient
import pandas as pd
import sys


spark = (SparkSession
	        .builder
	        .appName('example-pyspark-read-and-write-from-hive')
	        .enableHiveSupport()
			.getOrCreate())


hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')

my_table = str(sys.argv[1])
my_hdfs = str(sys.argv[2])
my_local = str(sys.argv[3])


df = spark.sql('SELECT * FROM ' + my_table)

print('---------------------------------------------------- ' + str(df.count()) + ' -----------------------------------------------------------------')

pdf = df.toPandas()
print(pdf.columns.values)
# correct a typo leng -> lang
# pdf = pdf.rename(columns={'city': 'city', 'country': 'country', 'created_at':'created_at', 'geo': 'geo', 'id':'id', 'leng' : 'lang', 'text': 'text', 'user':'user'})
print(pdf.columns.values)
my_data = pdf.to_json(orient='records')[1:-1].replace('},{', '}\n{') + '\n'
print(my_data)
hdfs.create_file(my_hdfs, my_data)

with open(my_local,'w') as file:
	file.write(my_data)
	file.close()

print("Data pre-procesing for stream data files are done, josn file has been generated!")