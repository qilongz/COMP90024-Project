# Clustering and Cloud Computing Assignment 2
## Twitter Analysis  
This repository contains all the scripts for Assignment 2 of subject COMP 90024, Clustering and Cloud Computing.  

The major task of this project can be divided into 5 components, for each the code can be found in the corresponding folders, folders can be found in folder assignment 2.

### Deployment    
#### Server Creation
The server creation has been scripted using boto scripts. There are two main scripts for this task. Deployment/server_deployment.py and Deployment/create_nodes.py.

Deployment/server_deployment.py creates our initial management server and 3 data servers. It also creates hosts and hosts.txt files outlining ip addresses and server names.

Deployment/create_nodes.py creates n addidtions servers (specified by parameter -n). It also creates new_hosts and new_hosts.txt files outlining ip addresses and server names.


### Twitter Harvest   
Twitter data harvest from Twitter API


### Spark   
Data preprocessing and analysis with MapReduce


### Twitter Explorer  
Deep analysis on elementary results

   
### Twitter Visualise  
Data visualisation with R 
