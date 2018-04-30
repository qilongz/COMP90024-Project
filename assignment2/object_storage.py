import boto


access_key = '89b0a2b8b2c44e009d375ea64205c1d3'
secret_key = 'd27e10a6b90c4935a3c7ee610246d681'

s3_conn = boto.connect_s3(
	aws_access_key_id=access_key,
	aws_secret_access_key=secret_key, 
	is_secure=True, 
	host='swik.rc.nectar.org.au',
	port=8888, 
	path='/')



#Create the bucket 
s3_conn.create_bucket('mybucket001',location = 'melbourne-qh2')
buckets = s3_conn.get_all_buckets()

for bucket in buckets:     
	print('Bucket name {} '.format(bucket.name)) 
