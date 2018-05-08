
import boto
from boto.s3.connection import S3Connection

ec2_access_key="89b0a2b8b2c44e009d375ea64205c1d3"
ec2_secret_key="d27e10a6b90c4935a3c7ee610246d681"

s3_conn	=	boto.connect_s3(aws_access_key_id=ec2_access_key,aws_secret_access_key=ec2_secret_key,is_secure=True,host='swift.rc.nectar.org.au',port=8888,path='/')
buckets     =       s3_conn.get_all_buckets()

for bucket  in      buckets:
    print('Bucket   name    {}      '.format(bucket.name))

