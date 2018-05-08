# sudo vi /etc/hosts
# sudo du -a / | sort -n -r | head -n 20
# df -h

sudo apt-get update

sudo mkdir /vdc
sudo mkfs.ext4 /dev/vdc
sudo mount /dev/vdc /vdc
sudo mkdir /vdc/usr
sudo mkdir /vdc/usr/hdp
sudo mkdir /vdc/hadoop
#sudo ln -s /vdc/hadoop/ /hadoop
sudo ln -s /vdc/usr/hdp/ /usr/hdp
sudo mkdir /vdc/usr/share
sudo mkdir /vdc/usr/share/dotnet
sudo ln -s /vdc/usr/share/dotnet/ /usr/share/dotnet

# need to change /etc/fstab
sudo bash -c "echo '/dev/vdc /vdc ext4 rw,relatime,data=ordered 0 0' >> /etc/fstab"

sudo wget -O /etc/apt/sources.list.d/ambari.list http://public-repo-1.hortonworks.com/ambari/ubuntu16/2.x/updates/2.6.1.5/ambari.list
sudo apt-key adv --recv-keys --keyserver keyserver.ubuntu.com B9733A7A07513CAD
sudo apt-get update

# sudo apt-get install libmysql-java
# cd /var/lib/ambari-server/resources/
# sudo ln -s /usr/share/java/mysql-connector-java.jar mysql-connector-java.jar

# cd ~

sudo apt-get -y install postgresql postgresql-contrib
sudo apt-get -y install libpostgresql-jdbc-java
#cd /var/lib/ambari-server/resources/
#sudo ln -s /usr/share/java/postgresql.jar postgresql.jar
#cd ~

# sudo ufw enable
# sudo ufw allow 22
# sudo ufw allow 9995
# sudo ufw allow 6080
# sudo ufw allow 5432
# sudo ufw allow 8080
# sudo ufw allow 8020
# sudo ufw allow from 115.146.86.0/24

sudo apt-get -y install ambari-server
sudo ambari-server setup
#sudo ambari-server setup --jdbc-db=mysql --jdbc-driver=/usr/share/java/mysql-connector-java.jar
sudo ambari-server setup --jdbc-db=postgres --jdbc-driver=/usr/share/java/postgresql.jar
sudo ambari-server start

## 
# sudo vi /etc/systemd/system/disable-thp.service
# sudo systemctl daemon-reload
# sudo systemctl start disable-thp
# sudo systemctl enable disable-thp

## remember to enable postgres connections
#sudo vi /etc/postgresql/9.5/main/pg_hba.conf
#sudo /etc/init.d/postgresql restart

# sudo apt-get update
# sudo apt-get install default-jdk
# sudo add-apt-repository ppa:webupd8team/java
# sudo apt-get update
# sudo apt-get install oracle-java8-installer
#sudo apt-get install postgresql postgresql-contrib
# CREATE USER ambari WITH PASSWORD 'T34m_f0rty!!';
# CREATE DATABASE ambari OWNER = ambari;
# CREATE USER hive WITH PASSWORD 'T34m_f0rty!!';
# CREATE DATABASE hive OWNER = hive;
# CREATE USER ambari WITH PASSWORD 'T34m_f0rty!!';
# CREATE DATABASE ambari OWNER = ambari;
# CREATE USER ranger WITH PASSWORD 'T34m_f0rty!!';
# CREATE DATABASE ranger OWNER = ranger;

#sudo -u postgres psql


curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'

sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.1.105

#sudo apt-get install -y openjdk-8-jdk-headless 

sudo apt-get update

# install hdfs3 package
sudo apt -y install python3-pip
echo "deb https://dl.bintray.com/wangzw/deb trusty contrib" | sudo tee /etc/apt/sources.list.d/bintray-wangzw-deb.list
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install libhdfs3 libhdfs3-dev

#sudo apt-get -o Dpkg::Options::="--force-confdef" -o Dpkg::Options::="--force-confold" install mypackage1 mypackage2


# sudo useradd -G team40 qilong

sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys E298A3A825C0D65DFD57CBB651716619E084DAB9
sudo add-apt-repository 'deb [arch=amd64,i386] https://cran.rstudio.com/bin/linux/ubuntu xenial/'
sudo apt-get update
sudo apt-get install r-base -y
sudo apt-get install libssl-dev -y
sudo apt-get install libxml2-dev -y
sudo apt-get install -y curl
sudo apt-get install -y httr
sudo apt-get install -y libcurl4-openssl-dev
sudo apt-get install -y libudunits2-dev
sudo apt-get install -y libpq-dev
sudo apt-get install -y libgdal-dev
sudo apt-get install -y libgeos-dev

sudo add-apt-repository ppa:ubuntugis/ubuntugis-unstable
sudo apt-get update
sudo apt-get install -y libudunits2-dev libgdal-dev libgeos-dev libproj-dev 
sudo apt-get install -y libcairo2-dev
sudo apt-get install -y libxt-dev

sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys E084DAB9
sudo add-apt-repository -y ppa:opencpu/jq
sudo apt-get update
sudo apt-get install -y libjq-dev
sudo apt-get install -y libprotobuf-dev
sudo apt-get install -y libv8-dev
sudo apt-get install -y protobuf-compiler 

sudo pip install pyspark




##############
#curl -i -H "X-Requested-By: ambari" -u admin:admin -X GET http://115.146.86.215:8080/api/v1/blueprints
#curl -i -H "X-Requested-By: ambari" -u admin:admin -X POST -d @blueprint.json http://115.146.86.215:8080/api/v1/blueprints/team40_cluster?validate_topology=false
#curl -i -H "X-Requested-By: ambari" -u admin:admin -X POST -d @test_blueprint.json http://115.146.86.215:8080/api/v1/blueprints/team40_cluster
#curl -i -H "X-Requested-By: ambari" -u admin:admin -X GET http://115.146.86.215:8080/api/v1/blueprints/team40_cluster