import boto
from boto.s3.key import Key
import hdfs
from hdfs import InsecureClient
access_key = '89b0a2b8b2c44e009d375ea64205c1d3'
secret_key = 'd27e10a6b90c4935a3c7ee610246d681'

s3_conn = boto.connect_s3(
	aws_access_key_id=access_key,
	aws_secret_access_key=secret_key, 
	is_secure=True, 
	host='swift.rc.nectar.org.au',
	port=8888, 
	path='/')

#Create the bucket 
#s3_conn.create_bucket('mybucket001',location = 'melbourne-qh2')
buckets = s3_conn.get_all_buckets()
hdfs = InsecureClient('http://115.146.86.32:50070', user='qilongz')
k=None
for bucket in buckets:     
	print('Bucket name {} '.format(bucket.name)) 
	k =  Key(bucket)
hdfs_path = "/team40/pack1_stream_data/"
dirt  = hdfs.walk(hdfs_path,2)


# Loop hdfs files and upload to object storage
for folder, dummy, filenames in dirt:
	#download
	for i in filenames:
		file_path = hdfs_path+i
		hdfs.download(file_path,"temp.json",overwrite=True, n_threads=5, temp_dir="temp_folder")
		k.key =  "team40/pack1_stream_data/" + i
		k.set_contents_from_filename("temp.json") 