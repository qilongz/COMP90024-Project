import sys
import boto
from boto.ec2.regioninfo import RegionInfo
import getopt
import time
import json
import logging
def wait_for_instance (ec2_conn, inst):
	res = None
	id = inst.instances[0].id
	while True:
		time.sleep(2)
		res=ec2_conn.get_all_reservations(instance_ids=id)
		state = res[0].instances[0].state
		if (state != "pending"):
			break
	return res

def wait_for_volume (ec2_conn, vol):
	curr_vols = None
	while True:
		time.sleep(2)
		curr_vols = ec2_conn.get_all_volumes([vol.id])	
		state = curr_vols[0].status
		if (state != "creating"):
			break
	return curr_vols

# start of function mainetn

def get_credentials(config):
	"""Read and return credentials from config file."""
	with open(config) as fp:
		jconfig = json.load(fp)

		# Attempt to read authentification details from config file.
		try:
			ec2_access_key = jconfig['ec2_access_key']
			ec2_secret_key = jconfig['ec2_secret_key']

		except Exception as e:
			logging.error(str(e))
			sys.exit()

		return ec2_access_key,ec2_secret_key




def main(argv):
	def print_help(file=sys.stdout):
		print('server_deployment.py -a <EC2 Access Key> -s <EC2 Secret Key> [-n <# of nodes>]', file=file)

	ec2_access_key = None
	ec2_secret_key = None
	num_nodes = None
	try:
		opts, args = getopt.getopt(argv[1:], "ha:s:n:", ["ec2AccessKey=", "ec2SecretKey="])
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
		elif opt in ("-n", "--num_of_nodes"):
			num_nodes = arg       

	hosts_file_content = """127.0.0.1       localhost

# The following lines are desirable for IPv6 capable hosts
::1     localhost       ip6-localhost   ip6-loopback
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters
"""

	hosts = ""


	if ec2_access_key == None or ec2_secret_key == None:
		ec2_access_key,ec2_secret_key = get_credentials('ec2_credential.json')

	if num_nodes == None:
		num_nodes = 1
	else:
		num_nodes = int(num_nodes)
	
	region = RegionInfo(name='melbourne', endpoint='nova.rc.nectar.org.au')
	ec2_conn = boto.connect_ec2(aws_access_key_id=ec2_access_key,aws_secret_access_key=ec2_secret_key,is_secure=True,region=region,port=8773,path='/services/Cloud',validate_certs=False)

	vol_mapping=""
	#Create instance with defualt value.
	for i in range(num_nodes):
		#'ami-00003a61'
		reservation = ec2_conn.run_instances('ami-00003a61',
			key_name='team40',
			instance_type='m1.medium',
			security_groups=['default','ssh','subnet_free_access','hadoop'],
			placement='melbourne-qh2')

		reservations = wait_for_instance (ec2_conn, reservation)

		hosts += "{ip}\t{host}-0.localdomain\t{host}-0\t#node{number}\n".format (host=reservations[0].id, ip=reservations[0].instances[0].private_ip_address, number=i)
		
		print('\nID: {r_id}\tStatus: {r_status}\tIP: {r_ip}\tPlacement: {r_placement}'.format(
			r_id=reservations[0].instances[0].id,
			r_status=reservations[0].instances[0].state,
			r_ip=reservations[0].instances[0].private_ip_address,
			r_placement=reservations[0].instances[0].placement))		
		
		
		
		#vol-632ca5ac	70	r-9arp1kfy-0
		#vol_req	= ec2_conn.create_volume(70,'melbourne-qh2')

		#vol_req = wait_for_volume (ec2_conn, vol_req)
		#print('Volume status: {}, volume AZ: {}'.format(vol_req[0].status, vol_req[0].zone))
		#ec2_conn.attach_volume(vol_req[0].id,reservations[0].instances[0].id,'/dev/vdc')	

		#vol-632ca5ac	70	r-9arp1kfy-0

	""" 		vol_mapping += '{}\t{}\t{}-0\n'.format(vol_req[0].id,70,reservations[0].id """
		
		
	hosts_file_content += hosts

	with open("./new_hosts", "w") as hosts_file:
		print(hosts_file_content, file=hosts_file)

	with open("./new_hosts.txt", "w") as host_list_file:
		print(hosts, file=host_list_file)

""" 	with open("./new_vol_mapping.txt", "w") as vol_map_file:
		print(vol_mapping, file=vol_map_file) """

	#print all reverations
	# reservations = ec2_conn.get_all_reservations()
	# for reservation in reservations:
	# 	# print(reservation)
	# 	print('\nID: {r_id}\tStatus: {r_status}\tIP: {r_ip}\tPlacement: {r_placement}'.format(
	# 		r_id=reservation.id,
	# 		r_status=reservation.instances[0].state,
	# 		r_ip=reservation.instances[0].private_ip_address,
	# 		r_placement=reservation.instances[0].placement)) 

# 
# end of function main
#





# call main function
if __name__ == "__main__":
	main(sys.argv)