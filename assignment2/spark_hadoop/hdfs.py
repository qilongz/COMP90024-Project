from pywebhdfs.webhdfs import PyWebHdfsClient

# create and write to a file

hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
test_file = open('/vdc/team40/nia_test/input.txt','r', encoding = 'utf-8')
my_data = test_file.read().encode('utf-8')
my_file = 'user/nikikiq/input.txt'
hdfs.create_file(my_file, my_data)


# append a file
# hdfs = PyWebHdfsClient(host='r-9arp1kfy-0.localdomain',port='50070', user_name='nikikiq')
# my_data = '01010101010101010101010101010101'
# my_file = 'user/nikikiq/python_test.txt'
# hdfs.append_file(my_file, my_data)


# alternative package 
# 
# https://creativedata.atlassian.net/wiki/spaces/SAP/pages/61177860/Python+-+Read+Write+files+from+HDFS
#
# https://pythonhosted.org/pywebhdfs/