from boto.s3.connection import S3Connection

ec2_access_key="xxx"
ec2_secret_key="xxx"

s3_conn	=	boto.connect_s3(aws_access_key_id=ec2_access_key,aws_secret_access_key=ec2_secret_key,is_secure=True,host='swift.rc.nectar.org.au',port=8888,path='/')
buckets     =       s3_conn.get_all_buckets()

for bucket  in      buckets:
    print('Bucket   name    {}      '.format(bucket.name))