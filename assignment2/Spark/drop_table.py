# Author: Yueni Chang 884622
# Date: 05/08/2018
# Clustering and Cloud Computing

# environment:
# export SPARK_HOME=/usr/hdp/current/spark2-client/
# export PYSPARK_PYTHON=python3
# usage:
# local: spark-submit /path/to/this .py/file   <ouput_table-name>
# cluster: spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  <ouput_table-name>
#
# This script is used to delete specific hive table 
# ---------------------------------------------------------------------------------------------------------------------------------------
from pyspark import SparkContext
from pyspark.sql import SparkSession
import sys

my_table = str(sys.argv[1])
spark = (SparkSession
	        .builder
	        .appName('example-pyspark-read-and-write-from-hive')
	        .enableHiveSupport()
			.getOrCreate())

spark.sql(('DROP TABLE IF EXISTS '+ my_table))