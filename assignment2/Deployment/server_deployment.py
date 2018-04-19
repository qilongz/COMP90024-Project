import sys
import boto
from boto.ec2.regioninfo import RegionInfo
import getopt

#
# start of function mainetn
#
def main(argv):
	def print_help(file=sys.stdout):
		print('server_deployment.py -a <EC2 Access Key> -s <EC2 Secret Key>', file=file)

	ec2_access_key = ""
	ec2_secret_key = ""
	try:
		opts, args = getopt.getopt(argv[1:], "ha:s:", ["ec2AccessKey=", "ec2SecretKey="])
	except getopt.GetoptError:
		print_help(file=sys.stderr)
		sys.exit(2)

	#print("opts:", opts, "args:", args)
	for opt, arg in opts:
		#print("option:", opt, "arg:", arg)
		if opt == '-h':
			print_help()
			sys.exit()
		elif opt in ("-a", "--ec2AccessKey"):
			ec2_access_key = arg
		elif opt in ("-s", "--ec2SecretKey"):
			ec2_secret_key = arg


	region = RegionInfo(name='melbourne', endpoint='nova.rc.nectar.org.au')
	ec2_conn = boto.connect_ec2(aws_access_key_id=ec2_access_key,
		aws_secret_access_key=ec2_secret_key,
		is_secure=True,
		region=region,
		port=8773,
		path='/services/Cloud',
		validate_certs=False)
	#images = ec2_conn.get_all_images()
	#for img in images:
	#	print('Image id: {id}, image name: {name}'.format(id=img.id, name=img.name))

	# reservations = ec2_conn.get_all_reservations()
	# print('Index\tID\t\tInstance')
	# for idx, res in enumerate(reservations):
	# 	print('{idx}\t{res_id}\t{res_inst}'.format(idx=idx, res_id=res.id, res_inst=res.instances))

	for i in range(1):
		reservation = ec2_conn.run_instances('ami-00003a61',
			key_name='team40',
			instance_type='m1.medium',
			security_groups=['default','ssh'],
			placement='melbourne-qh2')
		#'ami-00003a61'
#
# end of function main
#


# call main function
if __name__ == "__main__":
	main(sys.argv)




