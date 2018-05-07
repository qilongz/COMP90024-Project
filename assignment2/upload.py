
import hdfs
import os
from hdfs import InsecureClient


hdfs = InsecureClient('http://115.146.86.32:50070', user='qilongz')
directory_in_str = ""
directory = os.fsencode(directory_in_str)

for file in os.listdir(directory):
	filename = os.fsdecode(file)
	if filename.endswith(".json"): 
		# print(os.path.join(directory, filename))
		hdfs_path = "/team40/historical_data/"  + filename
		file_path = ""
		hdfs.upload(hdfs_path, file_path)
		