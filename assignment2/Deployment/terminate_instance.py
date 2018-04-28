
import sys
import boto
from boto.ec2.regioninfo import RegionInfo
import getopt
import time
import config
#
# start of function mainetn
#
def main(argv):
	def print_help(file=sys.stdout):
		print('server_deployment.py -a <EC2 Access Key> -s <EC2 Secret Key>', file=file)

	ec2_access_key = config.ec2AccessKey
	ec2_secret_key = config.ec2SecretKey
	# try:
	# 	opts, args = getopt.getopt(argv[1:], "ha:s:", ["ec2AccessKey=", "ec2SecretKey="])
	# except getopt.GetoptError:
	# 	print_help(file=sys.stderr)
	# 	sys.exit(2)

	# #print("opts:", opts, "args:", args)
	# for opt, arg in opts:
	# 	#print("option:", opt, "arg:", arg)
	# 	if opt == '-h':
	# 		print_help()
	# 		sys.exit()
	# 	elif opt in ("-a", "--ec2AccessKey"):
	# 		ec2_access_key = arg
	# 	elif opt in ("-s", "--ec2SecretKey"):
	# 		ec2_secret_key = arg


	region = RegionInfo(name='melbourne', endpoint='nova.rc.nectar.org.au')
	ec2_conn = boto.connect_ec2(aws_access_key_id=ec2_access_key,
		aws_secret_access_key=ec2_secret_key,
		is_secure=True,
		region=region,
		port=8773,
		path='/services/Cloud',
		validate_certs=False)


	reservations = ec2_conn.get_all_reservations()
	terminateID_list  = []
	volumeID_list = []
	for  res in reservations:
		terminateID_list.append(res.instances[0].id)
		print('\nID: {}\tIP: {}\tPlacement: {}'.format(res.instances[0].id,
		res.instances[0].private_ip_address,
		res.instances[0].placement)) 
	if terminateID_list:	
		ec2_conn.terminate_instances(instance_ids=terminateID_list)
	
	while True:
		time.sleep(2)
		curr_r = ec2_conn.get_all_reservations()	
		if (len(curr_r) == 0):
			print ('ALL instances been terminated')
			break



	volumnes = ec2_conn.get_all_volumes()
	print('Index\tID\t\tSize')
	for idx, vol in enumerate(volumnes):
		volumeID_list.append(vol.id)
		print('{}\t{}\t{}'.format(idx, vol.id,vol.size)) 
	if volumeID_list:
		for v_id in volumeID_list:
			ec2_conn.delete_volume(volume_id = v_id)
# call main function
if __name__ == "__main__":
	main(sys.argv)