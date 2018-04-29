import hdfs3
from hdfs3 import HDFileSystem

hdfs = HDFileSystem(host='vm-115-146-86-24.rc.cloud.unimelb.edu.au',port= 
50070, user_name='nikikiq')

with hdfs.open('/team40/myfile.txt', 'wb') as f:
    f.write(b'Hello, world!')