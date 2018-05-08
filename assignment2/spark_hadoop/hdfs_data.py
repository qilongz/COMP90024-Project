from pywebhdfs.webhdfs import PyWebHdfsClient

# create and write to a file

# read in file
hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')


file_01 = '/team40/stream_data/18-04-30-19-15-25-stream_*.json'
file_02 = '/team40/stream_data/18-04-30-19-24-00-stream_*.json'
file_03 = '/team40/stream_data/18-04-30-19-32-37-stream_*.json'
file_04 = '/team40/stream_data/18-04-30-19-41-20-stream_*.json'

data01 = hdfs.read_file(file_01)
data02 = hdfs.read_file(file_02)
data03 = hdfs.read_file(file_03)
data04 = hdfs.read_file(file_04)

my_file_01 = 'user/nikikiq/spark_test2/18-04-30-19-15-25-stream_*.json'
my_file_02 = 'user/nikikiq/spark_test2/18-04-30-19-24-00-stream_*.json'
my_file_03 = 'user/nikikiq/spark_test2/18-04-30-19-32-37-stream_*.json'
my_file_04 = 'user/nikikiq/spark_test2/18-04-30-19-41-20-stream_*.json'

hdfs.create_file(my_file_01, data01)
hdfs.create_file(my_file_02, data02)
hdfs.create_file(my_file_03, data03)
hdfs.create_file(my_file_04, data04)
