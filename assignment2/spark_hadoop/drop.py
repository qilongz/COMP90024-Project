from pyspark import SparkContext
from pyspark.sql import SparkSession


spark = (SparkSession
	        .builder
	        .appName('example-pyspark-read-and-write-from-hive')
	        .enableHiveSupport()
			.getOrCreate())

spark.sql('DROP TABLE IF EXISTS tweets_01')
spark.sql('DROP TABLE IF EXISTS tem_01')
# spark.sql('DROP TABLE twt_02')
# spark.sql('DROP TABLE twt_03')
# spark.sql('DROP TABLE twt_04 if exit')