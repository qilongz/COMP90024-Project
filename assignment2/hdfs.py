#from pywebhdfs.webhdfs import PyWebHdfsClient
import hdfs
from hdfs import InsecureClient

# create and write to a file

hdfs = InsecureClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
my_data = 'hello, world!'
my_file = 'user/nikikiq/python_test.txt'
hdfs.create_file(my_file, my_data)


# append a file
hdfs = InsecureClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
my_data = '01010101010101010101010101010101'
my_file = 'user/nikikiq/python_test.txt'
hdfs.append_file(my_file, my_data)


# alternative package 
# 
# https://creativedata.atlassian.net/wiki/spaces/SAP/pages/61177860/Python+-+Read+Write+files+from+HDFS
#
# https://pythonhosted.org/pywebhdfs/