from pywebhdfs.webhdfs import PyWebHdfsClient

# create and write to a file

# read in file
hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')


file_01 = '/team40/stream_data/18-04-30-18-35-14-stream_*.json'
file_02 = '/team40/stream_data/18-04-30-18-37-42-stream_*.json'

data01 = hdfs.read_file(file_01)
data02 = hdfs.read_file(file_02)


my_file_01 = 'user/nikikiq/spark_test/test/18-04-30-18-35-14-stream_*.json'
my_file_02 = 'user/nikikiq/spark_test/test/18-04-30-18-37-42-stream_*.json'


hdfs.create_file(my_file_01, data01)
hdfs.create_file(my_file_02, data02)