from pyspark import SparkContext
from pyspark.sql import SparkSession
import pandas

# We have to set the hive metastore uri.
SparkContext.setSystemProperty("hive.metastore.uris", "thrift://nn1:9083")

spark = (SparkSession
	        .builder
	        .appName('example-pyspark-read-and-write-from-hive')
	        .enableHiveSupport()
			.getOrCreate())


# We have to set the hive metastore uri.
# conf = SparkConf().setAppName("Data collection - Pyspark")       #setMaster("yarn")
# conf = conf.set("spark.hadoop.yarn.resourcemanager.address", "115.146.86.158:8088")
# sc = SparkContext(conf=conf)

# sqlContext = SQLContext(sc)
# hiveContext = HiveContext(sc)

df = spark.read.json('/user/nikikiq/spark_test/collections/twitterStream.json')
print(df.count())
df.write.mode('append').format('hive').saveAsTable('ts02')

# print(df.head(10))
# # Write into Hive
# df.write.saveAsTable('ts01')

# 'INSERT OVERWRITE TABLE ts01 SELECT DISTINCT * FROM ts01'
# Read from Hive
# 'CREATE TABLE ts04 AS SELECT DISTINCT * FROM ts02'
df1 = spark.sql('SELECT * FROM ts01')
df2 = spark.sql('SELECT * FROM ts02')
df4 = spark.sql('SELECT * FROM ts04')

pdf1 = df1.toPandas()
pdf2 = df2.toPandas()
pdf4 = df4.toPandas()

print(len(pdf1), len(pdf2), len(pdf4))
pdf1.show()
pdf2.show()
pdf4.show()

#print(df3.schema.names)