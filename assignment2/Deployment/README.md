### Deployment    
#### Server Creation
The server creation has been scripted using boto scripts. There are two main scripts for this task. Deployment/server_deployment.py and Deployment/create_nodes.py.

Deployment/server_deployment.py creates our initial management server and 3 data servers. It also creates hosts and hosts.txt files outlining ip addresses and server names. Usage: python3 server_deployment.py -a <EC2 Access Key> -s <EC2 Secret Key>

Deployment/create_nodes.py creates n addidtions servers (specified by parameter -n). It also creates new_hosts and new_hosts.txt files outlining ip addresses and server names of the new servers. Usage: create_nodes.py -a <EC2 Access Key> -s <EC2 Secret Key> [-n <# of nodes>]

Other boto scripts are management_node_deploy.py, which creates only the management node, and terminate_instance.py, which terminates all instances and volumes. 

#### Server Configuration
Server confisuration is done with Ansible Playbook using ansible/playbook.yml. There are three roles set up for this purpose: common, management and worker_node. Management server would have all three roles, and data nodes would have common and worker_node roles. Usage: ansible-playbook playbook.yml

The servers communicate to each other via fully qualified domain names, hence the host files on each server have been changed to map server names to IP addresses. The above boto script creates a hosts file of all the servers created, and we can move that file into the ansible/inventory/files folder for it to be distributed to the servers to be configured.

#### Hadoop deployment
Hadoop configuration is done with Ambari blueprint. The ansible/inventory/files/blueprint.json currently describes 5 groups of servers each with a component list of Hadoop packages to be installed. The video demo uses a simplified blueprint with two groups of servers. The blueprint is then submitted to Ambari web API using curl command.

Then ansible/inventory/files/cluster.json blueprint is submitted to Ambari to map actual servers to the blueprint groups and the deployment of components will begin.

The above operations are executed by Ansible using curl to submit the cluster deployment blueprints. Usage: ansible-playbook hadoop.yml

#### Backup
Backup is done using image and volumn backup in Nectar. We have a python boto script: Deployment/create_snapshot.py.
