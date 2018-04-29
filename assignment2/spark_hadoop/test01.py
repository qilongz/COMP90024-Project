import sys
 
from pyspark import SparkContext, SparkConf
 
if __name__ == "__main__":
 
  # create Spark context with Spark configuration
  conf = SparkConf().setAppName("Word Count - Python").set("spark.hadoop.yarn.resourcemanager.address", "115.146.86.158:8088")
  conf = conf.set("spark.hadoop.yarn.resourcemanager.hostname","115.146.86.158")
  conf = conf.set("spark.yarn.stagingDir", "hdfs://XXXXX:8020/user/hduser/")
  conf = conf.set("--deploy-mode", "yarn")
  sc = SparkContext(conf=conf)
 
  # read in text file and split each document into words
	# hdfs = PyWebHdfsClient(host='host',port='50070', user_name='hdfs')
	# my_file = 'user/hdfs/data/myfile.txt'
	# hdfs.read_file(my_file)

  words = sc.textFile('hdfs://r-9arp1kfy-0.localdomain:8020/user/nikikiq/input.txt').flatMap(lambda line: line.split(" "))
 
  # count the occurrence of each word
  wordCounts = words.map(lambda word: (word, 1)).reduceByKey(lambda a,b:a +b)
 
  wordCounts.saveAsTextFile("/user/nikikiq/nia_test/output/")
 