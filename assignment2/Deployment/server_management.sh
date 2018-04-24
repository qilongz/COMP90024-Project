# change /etc/hosts file

sudo apt-get update

sudo mkdir /vdc
sudo mkfs.ext4 /dev/vdc
sudo mount /dev/vdc /vdc
sudo mkdir /vdc/usr
sudo mkdir /vdc/usr/hdp
sudo mkdir /vdc/hadoop
sudo ln -s /vdc/hadoop/ /hadoop
sudo ln -s /vdc/usr/hdp/ /usr/hdp


sudo wget -O /etc/apt/sources.list.d/ambari.list http://public-repo-1.hortonworks.com/ambari/ubuntu16/2.x/updates/2.6.1.5/ambari.list
sudo apt-key adv --recv-keys --keyserver keyserver.ubuntu.com B9733A7A07513CAD
sudo apt-get update

# sudo apt-get install libmysql-java
# cd /var/lib/ambari-server/resources/
# sudo ln -s /usr/share/java/mysql-connector-java.jar mysql-connector-java.jar

# cd ~

sudo apt-get install postgresql postgresql-contrib
sudo apt-get install libpostgresql-jdbc-java
#cd /var/lib/ambari-server/resources/
#sudo ln -s /usr/share/java/postgresql.jar postgresql.jar
#cd ~

# sudo ufw enable
# sudo ufw allow 22
# sudo ufw allow 9995
# sudo ufw allow 6080
# sudo ufw allow 5432
# sudo ufw allow 8080
# sudo ufw allow from 115.146.86/24

sudo apt-get install ambari-server
sudo ambari-server setup
#sudo ambari-server setup --jdbc-db=mysql --jdbc-driver=/usr/share/java/mysql-connector-java.jar
sudo ambari-server setup --jdbc-db=postgres --jdbc-driver=/usr/share/java/postgresql.jar
sudo ambari-server start

## 
#sudo vi /etc/systemd/system/disable-thp.service
#sudo systemctl daemon-reload
#sudo systemctl start disable-thp
#sudo systemctl enable disable-thp

## remember to enable postgres connections
#sudo vi /etc/postgresql/9.5/main/pg_hba.conf
#sudo /etc/init.d/postgresql restart

# sudo apt-get update
# sudo apt-get install default-jdk
# sudo add-apt-repository ppa:webupd8team/java
# sudo apt-get update
# sudo apt-get install oracle-java8-installer
#sudo apt-get install postgresql postgresql-contrib
# CREATE USER hive WITH PASSWORD 'T34m_f0rty!!';
# CREATE DATABASE hive OWNER = hive;

#sudo -u postgres psql


