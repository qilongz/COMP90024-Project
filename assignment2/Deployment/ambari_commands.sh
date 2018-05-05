MGYzN2EyNDU0NjRkZWNl

curl -i -H "X-Requested-By: ambari" -u admin:T34m_f0rty!! -X GET http://115.146.86.24:8080/api/v1/blueprints
curl -i -H "X-Requested-By: ambari" -u admin:T34m_f0rty!! -X POST -d @blueprint.json http://115.146.86.24:8080/api/v1/blueprints/team40_cluster

curl -i -H "X-Requested-By: ambari" -u admin:admin -X POST -d @blueprint.json http://115.146.86.215:8080/api/v1/blueprints/team40_cluster?validate_topology=false
curl -i -H "X-Requested-By: ambari" -u admin:admin -X POST -d @cluster.json http://115.146.86.215:8080/api/v1/clusters/team40_cluster

curl -i -H "X-Requested-By: ambari" -u admin:admin -X POST -d @add_hosts.json http://115.146.86.215:8080/api/v1/clusters/team40_cluster/hosts