from pyspark import SparkContext
from pyspark.sql import SparkSession

# We have to set the hive metastore uri.

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

# df = spark.read.json('/user/nikikiq/spark_test/collections/twitterStream.json')
# print(df.count())
# df.write.mode('append').format('hive').saveAsTable('ts02')

# # print(df.head(10))
# # # Write into Hive
# df.write.saveAsTable('ts01')

# 'INSERT OVERWRITE TABLE ts01 SELECT DISTINCT * FROM ts01'
# Read from Hive
# 'CREATE TABLE ts04 AS SELECT DISTINCT * FROM ts02'


df2 = spark.sql('SELECT * FROM tweets_01')
df4 = df2.dropDuplicates(['id'])
df3 = spark.sql('SELECT * FROM tweets_01').dropDuplicates(['id'])

l1 = df2.rdd.map(lambda x: x['id']).collect()
l2 = df3.rdd.map(lambda x: x['id']).collect()

print(l1)
print(l2)
print(len(l1),len(l2))


l3 = list(set(l1))
l4 = list(set(l2))
print(len(l1),len(l2))

# df5 = spark.sql('SELECT * FROM twt_02 LEFT OUTER JOIN twt_03 ON (twt_03.twt_id = twt_02.twt_id) ')

# print('------------------------------------------------------------------------------------------------------------------------------------')
# print('-----------------------------------------------------after total df2: ' + str(df2.count()) + ' ------------------------------------------------')
# #76


# print('------------------------------------------------------------------------------------------------------------------------------------')
# #print(df3.take(50))
# print('-----------------------------------------------------after total df3: ' + str(df3.count()) + ' ------------------------------------------------')
# #2
# print('------------------------------------------------------------------------------------------------------------------------------------')
# print('-----------------------------------------------------after total df4: ' + str(df4.count()) + ' ------------------------------------------------')

# print(df3.take(50))
# print('-----------------------------------------------------after total df5: ' + str(df5.count()) + ' ------------------------------------------------')
# print(df3.schema.names)