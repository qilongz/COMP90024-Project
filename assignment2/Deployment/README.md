### Deployment    
#### Server Creation
The server creation has been scripted using boto scripts. There are two main scripts for this task. Deployment/server_deployment.py and Deployment/create_nodes.py.

Deployment/server_deployment.py creates our initial management server and 3 data servers. It also creates hosts and hosts.txt files outlining ip addresses and server names.

Deployment/create_nodes.py creates n addidtions servers (specified by parameter -n). It also creates new_hosts and new_hosts.txt files outlining ip addresses and server names of the new servers.

Other boto scripts are management_node_deploy.py, which creates only the management node, and terminate_instance.py, which terminates all instances and volumes.

#### Server Configuration
Server confisuration is done with Ansible Playbook using playbook.yml. There are three roles set up for this purpose: common, management and worker_node. Management server would have all three roles, and data nodes would have common and worker_node roles.

