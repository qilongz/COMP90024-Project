import sys
import boto
from boto.ec2.regioninfo import RegionInfo
import getopt
import time


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
#
def main(argv):
	def print_help(file=sys.stdout):
		print('server_deployment.py -a <EC2 Access Key> -s <EC2 Secret Key>', file=file)

	ec2_access_key = ""
	ec2_secret_key = ""
	try:
		opts,   args = getopt.getopt(argv[1:], "ha:s:", ["ec2AccessKey=", "ec2SecretKey="])
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

	hosts_file_content = """127.0.0.1       localhost

# The following lines are desirable for IPv6 capable hosts
::1     localhost       ip6-localhost   ip6-loopback
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters
"""

	hosts = ""

	region = RegionInfo(name='melbourne', endpoint='nova.rc.nectar.org.au')
	ec2_conn = boto.connect_ec2(aws_access_key_id=ec2_access_key,aws_secret_access_key=ec2_secret_key,is_secure=True,region=region,port=8773,path='/services/Cloud',validate_certs=False)

	#images = ec2_conn.get_all_images()
	#for img in images:
	#	print('Image id: {id}, image name: {name}'.format(id=img.id, name=img.name))

	# reservations = ec2_conn.get_all_reservations()
	# print('Index\tID\t\tInstance')
	# for idx, res in enumerate(reservations):
	# 	print('{idx}\t{res_id}\t{res_inst}'.format(idx=idx, res_id=res.id, res_inst=res.instances))

	### Run for the last instance with 40 volumne
	reservation = ec2_conn.run_instances('ami-00003a61',
		key_name='team40',
		instance_type='m1.medium',
		security_groups=['default','ssh','subnet_free_access','ambari','hadoop'],
		placement='melbourne-qh2')

	reservations = wait_for_instance (ec2_conn, reservation)
	
	hosts += "{ip}\t{host}-0.localdomain\t{host}-0\t#management\n".format (host=reservations[0].id, ip=reservations[0].instances[0].private_ip_address)

	print('\nID: {r_id}\tStatus: {r_status}\tIP: {r_ip}\tPlacement: {r_placement}'.format(
		r_id=reservations[0].instances[0].id,
		r_status=reservations[0].instances[0].state,
		r_ip=reservations[0].instances[0].private_ip_address,
		r_placement=reservations[0].instances[0].placement))		

	vol_req	= ec2_conn.create_volume(40,'melbourne-qh2')

	vol_req = wait_for_volume (ec2_conn, vol_req)
	print('Volume status: {}, volume AZ: {}'.format(vol_req[0].status, vol_req[0].zone))
	ec2_conn.attach_volume(vol_req[0].id,reservations[0].instances[0].id,'/dev/vdc')

	#Create instance with defualt value.
	for i in range(3):
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

		vol_req	= ec2_conn.create_volume(70,'melbourne-qh2')

		vol_req = wait_for_volume (ec2_conn, vol_req)
		print('Volume status: {}, volume AZ: {}'.format(vol_req[0].status, vol_req[0].zone))
		ec2_conn.attach_volume(vol_req[0].id,reservations[0].instances[0].id,'/dev/vdc')	
		
	hosts_file_content += hosts

	with open("./hosts", "w") as hosts_file:
		print(hosts_file_content, file=hosts_file)

	with open("./hosts.txt", "w") as host_list_file:
		print(hosts, file=host_list_file)

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




